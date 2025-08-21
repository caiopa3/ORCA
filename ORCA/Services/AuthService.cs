using MySql.Data.MySqlClient;
using System;

namespace ORCA.Services
{
    public class AuthService
    {
        private readonly string _connectionString;

        public AuthService(string servidor, string bd, string usr, string senha)
        {
            _connectionString = $"Server={servidor};Database={bd};User ID={usr};Password={senha};SslMode=none;";
        }

        /// <summary>
        /// Verifica se o email e senha são válidos.
        /// </summary>
        public bool ValidarLogin(string email, string senha)
        {
            using (var conexao = new MySqlConnection(_connectionString))
            {
                conexao.Open();

                string query = "SELECT COUNT(*) FROM usuario WHERE email = @Email AND senha = @Senha";
                using (var comando = new MySqlCommand(query, conexao))
                {
                    comando.Parameters.AddWithValue("@Email", email);
                    comando.Parameters.AddWithValue("@Senha", senha);

                    int count = Convert.ToInt32(comando.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        /// <summary>
        /// Retorna a permissão do usuário (adm, usr, ges) ou null se não existir.
        /// </summary>
        public string ObterPermissao(string email, string senha)
        {
            using (var conexao = new MySqlConnection(_connectionString))
            {
                conexao.Open();

                string query = "SELECT permissao FROM usuario WHERE email = @Email AND senha = @Senha LIMIT 1";
                using (var comando = new MySqlCommand(query, conexao))
                {
                    comando.Parameters.AddWithValue("@Email", email);
                    comando.Parameters.AddWithValue("@Senha", senha);

                    object result = comando.ExecuteScalar();
                    return result?.ToString();
                }
            }
        }
    }
}
