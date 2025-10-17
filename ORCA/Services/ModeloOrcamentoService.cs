using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ORCA.Services
{
    public class ModeloOrcamentoService
    {
        private readonly string _connectionString;

        public ModeloOrcamentoService(string servidor, string bd, string usr, string senha)
        {
            _connectionString = $"SERVER={servidor};PORT=3306;DATABASE={bd};UID={usr};PASSWORD={senha};";
        }

        /// <summary>
        /// Retorna o id do usuário pelo e-mail usando a view_decripto (retorna -1 se não encontrar).
        /// </summary>
        public int ObterUsuarioIdPorEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return -1;

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            using var cmd = new MySqlCommand("SELECT id FROM view_decripto WHERE email = @e LIMIT 1;", conn);
            cmd.Parameters.AddWithValue("@e", email.Trim());

            var r = cmd.ExecuteScalar();
            if (r == null || r == DBNull.Value) return -1;
            return Convert.ToInt32(r);
        }


        /// <summary>
        /// Cria o modelo, salva o JSON em modelo_orcamento_dados e compartilha com os usuários informados.
        /// Inclui FixedValues no JSON para suportar "valor fixo".
        /// </summary>
        public int CriarModeloOrcamento(
            string nomeModelo,
            int usrCriadorId,
            List<string> colunas,
            List<Dictionary<string, object>> linhas,
            List<int> usuariosCompartilhados,
            Dictionary<string, object> valoresFixos = null)
        {
            if (string.IsNullOrWhiteSpace(nomeModelo))
                throw new ArgumentException("nomeModelo é obrigatório.", nameof(nomeModelo));
            if (usrCriadorId <= 0)
                throw new ArgumentException("usrCriadorId inválido.", nameof(usrCriadorId));

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var tx = conn.BeginTransaction();

            try
            {
                // 1) modelo_orcamento
                using (var cmdModelo = new MySqlCommand(
                    "INSERT INTO modelo_orcamento (nome, usr_criador_id) VALUES (@n, @u);",
                    conn, tx))
                {
                    cmdModelo.Parameters.AddWithValue("@n", nomeModelo);
                    cmdModelo.Parameters.AddWithValue("@u", usrCriadorId);
                    cmdModelo.ExecuteNonQuery();
                    int modeloId = (int)cmdModelo.LastInsertedId;

                    // 2) JSON (colunas + linhas + fixed values)
                    var payload = new
                    {
                        Colunas = colunas ?? new List<string>(),
                        Linhas = linhas ?? new List<Dictionary<string, object>>(),
                        FixedValues = valoresFixos ?? new Dictionary<string, object>()
                    };
                    string json = JsonConvert.SerializeObject(payload);

                    using (var cmdDados = new MySqlCommand(
                        "INSERT INTO modelo_orcamento_dados (modelo_id, dados_json) VALUES (@m, @j);",
                        conn, tx))
                    {
                        cmdDados.Parameters.AddWithValue("@m", modeloId);
                        cmdDados.Parameters.AddWithValue("@j", json);
                        cmdDados.ExecuteNonQuery();
                    }

                    // 3) Compartilhamentos
                    if (usuariosCompartilhados != null && usuariosCompartilhados.Count > 0)
                    {
                        using var cmdUser = new MySqlCommand(
                            "INSERT INTO modelo_orcamento_usuarios (modelo_id, usuario_id) VALUES (@m, @u);",
                            conn, tx);
                        cmdUser.Parameters.Add("@m", MySqlDbType.Int32);
                        cmdUser.Parameters.Add("@u", MySqlDbType.Int32);

                        foreach (var uid in usuariosCompartilhados.Distinct())
                        {
                            cmdUser.Parameters["@m"].Value = modeloId;
                            cmdUser.Parameters["@u"].Value = uid;
                            cmdUser.ExecuteNonQuery();
                        }
                    }

                    tx.Commit();
                    return modeloId;
                }
            }
            catch
            {
                try { tx.Rollback(); } catch { /* ignore */ }
                throw;
            }
        }

        /// <summary>
        /// Overload compatível que aceita linhas como Dictionary<string,string>.
        /// </summary>
        public int CriarModeloOrcamento(
            string nomeModelo,
            int usrCriadorId,
            List<string> colunas,
            List<Dictionary<string, string>> linhas,
            List<int> usuariosCompartilhados,
            Dictionary<string, object> valoresFixos = null)
        {
            var linhasObj = (linhas ?? new List<Dictionary<string, string>>())
                .Select(row => row.ToDictionary(kv => kv.Key, kv => (object)kv.Value))
                .ToList();

            return CriarModeloOrcamento(nomeModelo, usrCriadorId, colunas, linhasObj, usuariosCompartilhados, valoresFixos);
        }

        /// <summary>
        /// Lista modelos criados por um determinado usuário (por id do criador).
        /// </summary>
        public List<(int Id, string Nome)> ListarModelosPorCriadorId(int usrCriadorId)
        {
            var lista = new List<(int, string)>();
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = new MySqlCommand("SELECT id, nome FROM modelo_orcamento WHERE usr_criador_id = @u ORDER BY id DESC;", conn);
            cmd.Parameters.AddWithValue("@u", usrCriadorId);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                lista.Add((r.GetInt32("id"), r.GetString("nome")));
            return lista;
        }

        /// <summary>
        /// Retorna o JSON armazenado em modelo_orcamento_dados para o modelo informado.
        /// </summary>
        public string ObterDadosJsonDoModelo(int modeloId)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = new MySqlCommand("SELECT dados_json FROM modelo_orcamento_dados WHERE modelo_id = @m LIMIT 1;", conn);
            cmd.Parameters.AddWithValue("@m", modeloId);
            var res = cmd.ExecuteScalar();
            return res?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Atualiza modelo: nome, json (colunas/linhas/fixedvalues) e compartilhamentos.
        /// Simples estratégia: deleta compartilhamentos antigos e insere os novos.
        /// </summary>
        public void AtualizarModeloOrcamento(
            int modeloId,
            string novoNome,
            List<string> colunas,
            List<Dictionary<string, object>> linhas,
            List<int> usuariosCompartilhados,
            Dictionary<string, object> valoresFixos = null)
        {
            if (modeloId <= 0) throw new ArgumentException("modeloId inválido.", nameof(modeloId));
            if (string.IsNullOrWhiteSpace(novoNome)) novoNome = "(sem nome)";

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var tx = conn.BeginTransaction();

            try
            {
                // 1) atualiza nome
                using (var cmd = new MySqlCommand("UPDATE modelo_orcamento SET nome = @n WHERE id = @id;", conn, tx))
                {
                    cmd.Parameters.AddWithValue("@n", novoNome);
                    cmd.Parameters.AddWithValue("@id", modeloId);
                    cmd.ExecuteNonQuery();
                }

                // 2) atualiza/insere dados JSON
                var payload = new
                {
                    Colunas = colunas ?? new List<string>(),
                    Linhas = linhas ?? new List<Dictionary<string, object>>(),
                    FixedValues = valoresFixos ?? new Dictionary<string, object>()
                };
                string json = JsonConvert.SerializeObject(payload);

                using (var cmd = new MySqlCommand("UPDATE modelo_orcamento_dados SET dados_json = @j WHERE modelo_id = @id;", conn, tx))
                {
                    cmd.Parameters.AddWithValue("@j", json);
                    cmd.Parameters.AddWithValue("@id", modeloId);
                    var affected = cmd.ExecuteNonQuery();
                    if (affected == 0)
                    {
                        using var ins = new MySqlCommand("INSERT INTO modelo_orcamento_dados (modelo_id, dados_json) VALUES (@id, @j);", conn, tx);
                        ins.Parameters.AddWithValue("@id", modeloId);
                        ins.Parameters.AddWithValue("@j", json);
                        ins.ExecuteNonQuery();
                    }
                }

                // 3) atualiza compartilhamentos: remove antigos e insere os novos
                using (var del = new MySqlCommand("DELETE FROM modelo_orcamento_usuarios WHERE modelo_id = @id;", conn, tx))
                {
                    del.Parameters.AddWithValue("@id", modeloId);
                    del.ExecuteNonQuery();
                }

                if (usuariosCompartilhados != null && usuariosCompartilhados.Count > 0)
                {
                    using var cmdUser = new MySqlCommand("INSERT INTO modelo_orcamento_usuarios (modelo_id, usuario_id) VALUES (@m, @u);", conn, tx);
                    cmdUser.Parameters.Add("@m", MySqlDbType.Int32);
                    cmdUser.Parameters.Add("@u", MySqlDbType.Int32);

                    foreach (var uid in usuariosCompartilhados.Distinct())
                    {
                        cmdUser.Parameters["@m"].Value = modeloId;
                        cmdUser.Parameters["@u"].Value = uid;
                        cmdUser.ExecuteNonQuery();
                    }
                }

                tx.Commit();
            }
            catch
            {
                try { tx.Rollback(); } catch { /* ignore */ }
                throw;
            }
        }

        /// <summary>
        /// Exclui um modelo por id (remove registros relacionados também).
        /// ATENÇÃO: operação destrutiva — use confirmação na UI antes de chamar.
        /// </summary>
        public void ExcluirModelo(int modeloId)
        {
            if (modeloId <= 0) return;

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var tx = conn.BeginTransaction();

            try
            {
                // remover compartilhamentos
                using (var delUsers = new MySqlCommand("DELETE FROM modelo_orcamento_usuarios WHERE modelo_id = @id;", conn, tx))
                {
                    delUsers.Parameters.AddWithValue("@id", modeloId);
                    delUsers.ExecuteNonQuery();
                }

                // remover dados do modelo
                using (var delDados = new MySqlCommand("DELETE FROM modelo_orcamento_dados WHERE modelo_id = @id;", conn, tx))
                {
                    delDados.Parameters.AddWithValue("@id", modeloId);
                    delDados.ExecuteNonQuery();
                }

                // remover registro do modelo
                using (var delModelo = new MySqlCommand("DELETE FROM modelo_orcamento WHERE id = @id;", conn, tx))
                {
                    delModelo.Parameters.AddWithValue("@id", modeloId);
                    delModelo.ExecuteNonQuery();
                }

                tx.Commit();
            }
            catch
            {
                try { tx.Rollback(); } catch { }
                throw;
            }
        }

        /// <summary>
        /// Adiciona usuários a um modelo existente (utilitário).
        /// </summary>
        public void AdicionarUsuariosAoModelo(int modeloId, IEnumerable<int> usuarios)
        {
            if (modeloId <= 0 || usuarios == null) return;

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            using var cmd = new MySqlCommand(
                "INSERT INTO modelo_orcamento_usuarios (modelo_id, usuario_id) VALUES (@m, @u);", conn);
            cmd.Parameters.Add("@m", MySqlDbType.Int32);
            cmd.Parameters.Add("@u", MySqlDbType.Int32);

            foreach (var uid in usuarios.Distinct())
            {
                cmd.Parameters["@m"].Value = modeloId;
                cmd.Parameters["@u"].Value = uid;
                cmd.ExecuteNonQuery();
            }
        }
    }
}
