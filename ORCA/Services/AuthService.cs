using MySql.Data.MySqlClient;
using System;

namespace ORCA.Services
{
    public class AuthService
    {
        private readonly string _connectionString;

        public AuthService(string servidor, string bd, string usr, string senha)
        {
            _connectionString = $"SERVER={servidor}; PORT=3306; DATABASE={bd}; UID={usr}; PASSWORD={senha};";
        }

        public bool ValidarLogin(string email, string password)
        {
            using (var conexao = new MySqlConnection(_connectionString))
            {
                conexao.Open();
                string query = "SELECT COUNT(*) FROM usuario WHERE email = @email AND senha = @password";
                using (var comando = new MySqlCommand(query, conexao))
                {
                    comando.Parameters.AddWithValue("@email", email);
                    comando.Parameters.AddWithValue("@password", password);

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
                string query = "SELECT permissao FROM usuario WHERE email = @email AND senha = @password";
                using (var comando = new MySqlCommand(query, conexao))
                {
                    comando.Parameters.AddWithValue("@email", email);
                    comando.Parameters.AddWithValue("@password", password);

                    object resultado = comando.ExecuteScalar();
                    return resultado?.ToString() ?? "";
                }
            }
        }
    }
}
