using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ORCA.Services
{
    public class ModeloOrcamentoService
    {
        private readonly string _connectionString;

        public ModeloOrcamentoService(string servidor, string bd, string usr, string senha)
        {
            _connectionString = $"SERVER={servidor}; PORT=3306; DATABASE={bd}; UID={usr}; PASSWORD={senha};";
        }

        /// <summary>
        /// Retorna o id do usuário pelo e-mail. Retorna -1 se não encontrar.
        /// </summary>
        public int ObterUsuarioIdPorEmail(string email)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = new MySqlCommand("SELECT id FROM usuario WHERE email=@e LIMIT 1;", conn);
            cmd.Parameters.AddWithValue("@e", email);
            var r = cmd.ExecuteScalar();
            if (r == null || r == DBNull.Value) return -1;
            return Convert.ToInt32(r);
        }

        /// <summary>
        /// Cria o modelo, salva o JSON em modelo_orcamento_dados e compartilha com os usuários informados.
        /// Agora inclui FixedValues no JSON para suportar "valor fixo" ao adicionar novas linhas.
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
        /// Método utilitário caso precise adicionar usuários a um modelo já criado.
        /// </summary>
        public void AdicionarUsuariosAoModelo(int modeloId, IEnumerable<int> usuarios)
        {
            if (modeloId <= 0 || usuarios == null) return;

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            using var cmd = new MySqlCommand(
                "INSERT INTO modelo_orcamento_usuarios (modelo_id, usuario_id) VALUES (@m, @u);",
                conn);
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
