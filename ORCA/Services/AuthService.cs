using MySql.Data.MySqlClient;
using System;

namespace ORCA.Services
{
    public class AuthService
    {
        private readonly string _connectionString;

        public AuthService(string servidor, string banco, string usuario, string senha)
        {
            _connectionString = $"Server={servidor};Database={banco};Uid={usuario};Pwd={senha};";
        }

        public bool ValidarLogin(string email, string senha)
        {
            using (var conexao = new MySqlConnection(_connectionString))
            {
                conexao.Open();

                string query = "SELECT COUNT(*) FROM usuario WHERE email = @Email AND senha = @Senha";
                using (var cmd = new MySqlCommand(query, conexao))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Senha", senha);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        public string? ObterPermissao(string email, string senha)
        {
            using (var conexao = new MySqlConnection(_connectionString))
            {
                conexao.Open();

                string query = "SELECT permissao FROM usuario WHERE email = @Email AND senha = @Senha LIMIT 1";
                using (var cmd = new MySqlCommand(query, conexao))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Senha", senha);

                    object result = cmd.ExecuteScalar();
                    return result?.ToString();
                }
            }
        }
    }
}
