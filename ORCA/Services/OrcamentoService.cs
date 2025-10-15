using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Linq;
using System.Globalization;

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

        /// <summary>
        /// Carrega o JSON para este orçamento. Primeiro tenta orcamento_dados (dados já salvos pelo usuário),
        /// se não encontrar, retorna o JSON base do modelo (modelo_orcamento_dados).
        /// </summary>
        public string CarregarModeloJsonPorOrcamentoId(int orcamentoId, string email)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            // 1) Primeiro tenta carregar os dados específicos do orçamento (orcamento_dados)
            string sqlOrc = @"
                SELECT od.dados_json
                FROM orcamento_dados od
                INNER JOIN orcamento o ON od.orcamento_id = o.id
                WHERE o.id = @id
                  AND o.usr_email = @Email
                LIMIT 1;
            ";

            using (var cmdOrc = new MySqlCommand(sqlOrc, conn))
            {
                cmdOrc.Parameters.AddWithValue("@id", orcamentoId);
                cmdOrc.Parameters.AddWithValue("@Email", email);

                var result = cmdOrc.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                    return result.ToString();
            }

            // 2) Caso não exista em orcamento_dados, usa o JSON base do modelo
            string sqlModelo = @"
                SELECT mod_dados.dados_json
                FROM orcamento o
                INNER JOIN modelo_orcamento_dados mod_dados ON mod_dados.modelo_id = o.modelo_id
                WHERE o.id = @id
                  AND o.usr_email = @Email
                LIMIT 1;
            ";

            using (var cmdModelo = new MySqlCommand(sqlModelo, conn))
            {
                cmdModelo.Parameters.AddWithValue("@id", orcamentoId);
                cmdModelo.Parameters.AddWithValue("@Email", email);

                var result = cmdModelo.ExecuteScalar();
                return result?.ToString() ?? string.Empty;
            }
        }


        /// <summary>
        /// Converte o JSON do modelo (ou orcamento_dados) para DataTable.
        /// Se o JSON incluir "FixedValues", aplica DataColumn.DefaultValue para cada coluna correspondente.
        /// </summary>
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
                // fallback se o JSON for um array de linhas
                var fallback = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(json);
                if (fallback == null || fallback.Count == 0) return dt;

                foreach (var col in fallback[0].Keys)
                    dt.Columns.Add(RemoverTipo(col));

                foreach (var row in fallback)
                {
                    var r = dt.NewRow();
                    int i = 0;
                    foreach (var val in row.Values)
                    {
                        r[i++] = val ?? "";
                    }
                    dt.Rows.Add(r);
                }

                return dt;
            }

            // Extrai colunas, linhas e fixed values do objeto
            var colunas = obj["Colunas"]?.Select(c => RemoverTipo(c.ToString())).ToList() ?? new List<string>();
            var linhas = obj["Linhas"] as JArray ?? new JArray();

            // Se não houver "Colunas", tenta inferir pelas chaves da primeira linha
            if (colunas.Count == 0 && linhas.Count > 0 && linhas[0] is JObject firstRow)
                colunas = firstRow.Properties().Select(p => RemoverTipo(p.Name)).ToList();

            // Lê FixedValues (se existir) em dicionário case-insensitive
            var fixedValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            if (obj["FixedValues"] is JObject fvObj)
            {
                foreach (var prop in fvObj.Properties())
                {
                    var key = RemoverTipo(prop.Name);
                    var token = prop.Value;
                    // tenta parse numérico primeiro
                    if (token != null && token.Type == JTokenType.Float || token.Type == JTokenType.Integer)
                    {
                        fixedValues[key] = token.ToObject<double>();
                    }
                    else
                    {
                        var s = token?.ToString();
                        if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out double dInv))
                            fixedValues[key] = dInv;
                        else if (double.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out double dCurr))
                            fixedValues[key] = dCurr;
                        else
                            fixedValues[key] = s ?? "";
                    }
                }
            }

            // Cria as colunas no DataTable e já aplica DefaultValue quando existir fixedValues
            foreach (var col in colunas)
            {
                var clean = RemoverTipo(col);
                if (!dt.Columns.Contains(clean))
                    dt.Columns.Add(clean);

                if (fixedValues.TryGetValue(clean, out var fv))
                {
                    try
                    {
                        // se fv for número em double, use ele; caso contrário, mantenha string
                        dt.Columns[clean].DefaultValue = fv ?? "";
                    }
                    catch
                    {
                        dt.Columns[clean].DefaultValue = fv?.ToString() ?? "";
                    }
                }
            }

            // Preenche as linhas
            foreach (var item in linhas.OfType<JObject>())
            {
                var row = dt.NewRow();
                foreach (var prop in item.Properties())
                {
                    var colName = RemoverTipo(prop.Name);
                    if (!dt.Columns.Contains(colName))
                        dt.Columns.Add(colName);

                    row[colName] = prop.Value?.ToString() ?? string.Empty;
                }

                // se alguma coluna faltou no JSON desta linha, o DefaultValue da coluna já cuidará
                dt.Rows.Add(row);
            }

            return dt;
        }

        private string RemoverTipo(string coluna)
        {
            return Regex.Replace(coluna, @"\s*\(.*?\)", ""); // remove "(Texto)", "(Número)" etc.
        }

        public DataTable CarregarUsuarios()
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString)) // usa a connectionString vinda do construtor
            {
                conn.Open();

                string query = "SELECT id, email, permissao FROM usuario WHERE permissao IN ('usr', 'ges')";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt;
            }
        }

        public List<string> ListarPermissoes()
        {
            var permissoes = new List<string>();

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            string sql = "SELECT DISTINCT permissao FROM usuario;";
            using var cmd = new MySqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                permissoes.Add(reader.GetString("permissao"));
            }

            return permissoes;
        }

        public void CadastrarUsuarioCompleto(
            string nomeCompleto,    
            string email,
            string senha,
            string telefoneCelular,
            string permissao,
            string cpf,
            string rg
            )
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            // <IMPORTANTE> adapte os nomes das colunas conforme seu schema.
            string sql = @"
        INSERT INTO usuario
        (nome_completo, email, senha, telefone_celular,
         permissao, cpf, rg)
        VALUES
        (@nome_completo, @Email, @Senha, @telefone_celular,
         @permissao, @cpf, @rg);
    ";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@nome_completo", (object)nomeCompleto ?? "");
            cmd.Parameters.AddWithValue("@Email", (object)email ?? "");
            cmd.Parameters.AddWithValue("@Senha", (object)senha ?? "");
            cmd.Parameters.AddWithValue("@telefone_celular", (object)telefoneCelular ?? "");
            cmd.Parameters.AddWithValue("@permissao", (object)permissao ?? "usr");
            cmd.Parameters.AddWithValue("@cpf", (object)cpf ?? "");
            cmd.Parameters.AddWithValue("@rg", (object)rg ?? "");
            cmd.ExecuteNonQuery();
        }


        public class PermissaoItem
        {
            public string Valor { get; set; } // valor real que vai para o banco
            public string Texto { get; set; } // o que aparece na ComboBox

            public override string ToString()
            {
                return Texto; // é o que será exibido no ComboBox
            }
        }

        public DataTable FiltrarUsuarios(string campo, string valor)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            // monta a query de filtro
            string query = $"SELECT id, email, permissao FROM usuario WHERE permissao IN ('usr','ges') AND {campo} LIKE @valor";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@valor", "%" + valor + "%");

            var adapter = new MySqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);

            return dt;
        }

        public void SalvarDadosOrcamento(int orcamentoId, DataTable tabela, int usuarioId, Dictionary<string, string> formulas)
        {
            var colunas = tabela.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();

            var linhas = new List<Dictionary<string, object>>();
            foreach (DataRow dr in tabela.Rows)
            {
                var dict = new Dictionary<string, object>();
                foreach (DataColumn col in tabela.Columns)
                    dict[col.ColumnName] = dr[col];
                linhas.Add(dict);
            }

            var obj = new { Colunas = colunas, Linhas = linhas, Formulas = formulas };
            string json = JsonConvert.SerializeObject(obj);

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            string checkSql = "SELECT id FROM orcamento_dados WHERE orcamento_id=@id AND usuario_id=@uid";
            using var checkCmd = new MySqlCommand(checkSql, conn);
            checkCmd.Parameters.AddWithValue("@id", orcamentoId);
            checkCmd.Parameters.AddWithValue("@uid", usuarioId);

            var existe = checkCmd.ExecuteScalar();

            if (existe == null)
            {
                string insertSql = "INSERT INTO orcamento_dados (orcamento_id, usuario_id, dados_json) VALUES (@id, @uid, @json)";
                using var cmd = new MySqlCommand(insertSql, conn);
                cmd.Parameters.AddWithValue("@id", orcamentoId);
                cmd.Parameters.AddWithValue("@uid", usuarioId);
                cmd.Parameters.AddWithValue("@json", json);
                cmd.ExecuteNonQuery();
            }
            else
            {
                string updateSql = "UPDATE orcamento_dados SET dados_json=@json WHERE orcamento_id=@id AND usuario_id=@uid";
                using var cmd = new MySqlCommand(updateSql, conn);
                cmd.Parameters.AddWithValue("@id", orcamentoId);
                cmd.Parameters.AddWithValue("@uid", usuarioId);
                cmd.Parameters.AddWithValue("@json", json);
                cmd.ExecuteNonQuery();
            }
        }

        public int ObterUsuarioIdPorEmail(string email)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            string sql = "SELECT id FROM usuario WHERE email=@e";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@e", email);

            var result = cmd.ExecuteScalar();
            return result == null ? 0 : Convert.ToInt32(result);
        }

        public void AtualizarUsuario(string emailAntigo, string novoEmail, string novaSenha)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            string sql = "UPDATE usuario SET email=@NovoEmail, senha=@Senha WHERE email=@EmailAntigo";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@NovoEmail", novoEmail);
            cmd.Parameters.AddWithValue("@Senha", novaSenha);
            cmd.Parameters.AddWithValue("@EmailAntigo", emailAntigo);

            cmd.ExecuteNonQuery();
        }
        public bool VerificarSenhaAtual(string email, string senha)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            string sql = "SELECT COUNT(*) FROM usuario WHERE email=@Email AND senha=@Senha";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Senha", senha);

            int count = Convert.ToInt32(cmd.ExecuteScalar());
            return count > 0;
        }
        public DataRow BuscarUsuarioPorEmail(string email)
        {
            string sql = "SELECT * FROM usuario WHERE email = @Email";
            using var conn = new MySqlConnection(_connectionString);
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Email", email);
            using var da = new MySqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        public void AtualizarUsuarioCompleto(string email, string nomeCompleto, string telefone, string cpf, string rg, string permissao)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            string sql = @"
        UPDATE usuario
        SET nome_completo=@Nome, telefone_celular=@Telefone,
            cpf=@CPF, rg=@RG, permissao=@Permissao
        WHERE email=@Email
    ";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Nome", nomeCompleto);
            cmd.Parameters.AddWithValue("@Telefone", telefone);
            cmd.Parameters.AddWithValue("@CPF", cpf);
            cmd.Parameters.AddWithValue("@RG", rg);
            cmd.Parameters.AddWithValue("@Permissao", permissao);
            cmd.Parameters.AddWithValue("@Email", email);

            cmd.ExecuteNonQuery();
        }
    }
}
