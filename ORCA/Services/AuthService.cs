using MySql.Data.MySqlClient;
using System;

namespace ORCA.Services
{
    public class AuthService
    {
        private readonly string _connectionString;

        public AuthService(string servidor, string bd, string usr, string senha)
        {
            // Se as variáveis de ambiente do GitHub Actions existirem, usa elas
            string envHost = Environment.GetEnvironmentVariable("MYSQL_HOST");
            string envDb   = Environment.GetEnvironmentVariable("MYSQL_DATABASE");
            string envUser = Environment.GetEnvironmentVariable("MYSQL_USER");
            string envPass = Environment.GetEnvironmentVariable("MYSQL_PASSWORD");

            servidor = !string.IsNullOrEmpty(envHost) ? envHost : servidor;
            bd       = !string.IsNullOrEmpty(envDb)   ? envDb   : bd;
            usr      = !string.IsNullOrEmpty(envUser) ? envUser : usr;
            senha    = !string.IsNullOrEmpty(envPass) ? envPass : senha;

            _connectionString = $"SERVER={servidor};PORT=3306;DATABASE={bd};UID={usr};PASSWORD={senha};";
        }

        public bool ValidarLogin(string email, string password)
        {
            using (var conexao = new MySqlConnection(_connectionString))
            {
                conexao.Open();

                string query = "SELECT COUNT(*) FROM usuario WHERE email = @Email AND senha = @Senha";
                using (var comando = new MySqlCommand(query, conexao))
                {
                    comando.Parameters.AddWithValue("@Email", email);
                    comando.Parameters.AddWithValue("@Senha", password);

                    int count = Convert.ToInt32(comando.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        public string ObterPermissao(string email, string password)
        {
            using (var conexao = new MySqlConnection(_connectionString))
            {
                conexao.Open();

                string query = "SELECT permissao FROM usuario WHERE email = @Email AND senha = @Senha";
                using (var comando = new MySqlCommand(query, conexao))
                {
                    comando.Parameters.AddWithValue("@Email", email);
                    comando.Parameters.AddWithValue("@Senha", password);

                    object result = comando.ExecuteScalar();
                    return result?.ToString() ?? string.Empty;
                }
            }
        }
    }
}

}

}
