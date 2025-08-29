using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;

namespace ORCA.Services
{
    public class OrcamentoService
    {
        private readonly string _connectionString;

        public OrcamentoService(string servidor, string bd, string usr, string senha)
        {
            _connectionString = $"SERVER={servidor};PORT=3306;DATABASE={bd};UID={usr};PASSWORD={senha};";
        }

        public int InserirOrcamento(string nome, string emailUsuario)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            // seu schema usa usr_email
            var cmd = new MySqlCommand(
                "INSERT INTO orcamento (nome, usr_email) VALUES (@n, @e); SELECT LAST_INSERT_ID();",
                conn);
            cmd.Parameters.AddWithValue("@n", nome);
            cmd.Parameters.AddWithValue("@e", emailUsuario);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void AtualizarNome(int id, string novoNome)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand(
                "UPDATE orcamento SET nome=@n WHERE id=@id",
                conn);
            cmd.Parameters.AddWithValue("@n", novoNome);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public List<(int Id, string Nome)> ListarPorEmail(string emailUsuario)
        {
            var lista = new List<(int, string)>();

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand(
                "SELECT id, nome FROM orcamento WHERE usr_email=@e",
                conn);
            cmd.Parameters.AddWithValue("@e", emailUsuario);

            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                lista.Add((r.GetInt32("id"), r.GetString("nome")));
            }

            return lista;
        }

        // Retorna lista de modelos disponíveis para o usuário
        public List<SelecionarModeloWindow.ModeloItem> ListarModelosDisponiveis(string email)
        {
            var modelos = new List<SelecionarModeloWindow.ModeloItem>();

            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string sql = @"
            SELECT m.id, m.nome
            FROM modelo_orcamento m
            JOIN modelo_orcamento_usuarios um ON m.id = um.modelo_id
            JOIN usuario u ON u.id = um.usuario_id
            WHERE u.email = @Email";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            modelos.Add(new SelecionarModeloWindow.ModeloItem
                            {
                                Id = reader.GetInt32("id"),
                                Nome = reader.GetString("nome")
                            });
                        }
                    }
                }
            }

            return modelos;
        }

        // Insere orçamento já ligado ao modelo
        public int InserirOrcamentoComModelo(string nome, string email, int modeloId)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "INSERT INTO orcamento (nome, usr_email, modelo_id) VALUES (@Nome, @Email, @ModeloId); SELECT LAST_INSERT_ID();";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Nome", nome);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@ModeloId", modeloId);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public string CarregarModeloJsonPorOrcamentoId(int orcamentoId)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            const string sql = @"
                SELECT mod_dados.dados_json
                FROM orcamento o
                INNER JOIN modelo_orcamento_dados mod_dados ON mod_dados.modelo_id = o.modelo_id
                WHERE o.id = @id;";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", orcamentoId);

            var result = cmd.ExecuteScalar();
            return result?.ToString() ?? string.Empty;
        }

        public DataTable ModeloJsonParaDataTable(string json)
        {
            var dt = new DataTable();

            if (string.IsNullOrWhiteSpace(json))
                return dt;

            JObject obj;
            try
            {
                obj = JObject.Parse(json);
            }
            catch
            {
                // fallback se por acaso o JSON já estiver como array de linhas
                var fallback = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(json);
                if (fallback == null || fallback.Count == 0) return dt;

                foreach (var col in fallback[0].Keys)
                    dt.Columns.Add(col);

                foreach (var row in fallback)
                    dt.Rows.Add(row.Values.ToArray());

                return dt;
            }

            var colunas = obj["Colunas"]?.Select(c => c.ToString()).ToList() ?? new List<string>();
            var linhas = obj["Linhas"] as JArray ?? new JArray();

            // Se não houver "Colunas", tenta inferir pelas chaves da primeira linha
            if (colunas.Count == 0 && linhas.Count > 0 && linhas[0] is JObject firstRow)
                colunas = firstRow.Properties().Select(p => p.Name).ToList();

            // Cria as colunas no DataTable
            foreach (var col in colunas)
                if (!dt.Columns.Contains(col))
                    dt.Columns.Add(col);

            // Adiciona as linhas
            foreach (var item in linhas.OfType<JObject>())
            {
                var row = dt.NewRow();
                foreach (var prop in item.Properties())
                {
                    var colName = prop.Name;
                    if (!dt.Columns.Contains(colName))
                        dt.Columns.Add(colName);

                    row[colName] = prop.Value?.ToString() ?? string.Empty;
                }
                dt.Rows.Add(row);
            }

            return dt;
        }

        public void SalvarDadosOrcamento(int orcamentoId, string jsonAtualizado)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            // 1) Descobre qual modelo está vinculado a este orçamento
            var getModeloCmd = new MySqlCommand("SELECT modelo_id FROM orcamento WHERE id=@id", conn);
            getModeloCmd.Parameters.AddWithValue("@id", orcamentoId);
            var modeloIdObj = getModeloCmd.ExecuteScalar();

            if (modeloIdObj == null || modeloIdObj == DBNull.Value)
                throw new Exception("Este orçamento não está vinculado a nenhum modelo.");

            int modeloId = Convert.ToInt32(modeloIdObj);

            // 2) Atualiza o JSON dos dados do orçamento baseado no modelo
            var updateCmd = new MySqlCommand(
                "UPDATE modelo_orcamento_dados SET dados_json=@json WHERE modelo_id=@modeloId",
                conn);
            updateCmd.Parameters.AddWithValue("@json", jsonAtualizado);
            updateCmd.Parameters.AddWithValue("@modeloId", modeloId);
            updateCmd.ExecuteNonQuery();
        }


    }
}
