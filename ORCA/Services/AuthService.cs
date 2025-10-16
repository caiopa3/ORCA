using MySql.Data.MySqlClient;
using System;

namespace ORCA.Services
{
    public class AuthService
    {
        private readonly string _connectionString;

        public AuthService(string servidor, string bd, string usr, string senha)
        {
            _connectionString = $"SERVER={servidor};PORT=3306;DATABASE={bd};UID={usr};PASSWORD={senha};";
        }

        // Método novo (que você já vinha usando nos testes anteriores)
        public bool Login(string email, string senha)
        {
            return ValidarLogin(email, senha);
        }

        // Compatibilidade com código antigo
        public bool ValidarLogin(string email, string senha)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand(
                "SELECT COUNT(*) FROM view_decripto WHERE email=@e AND senha=@s",
                conn);
            cmd.Parameters.AddWithValue("@e", email);
            cmd.Parameters.AddWithValue("@s", senha);
            var count = Convert.ToInt32(cmd.ExecuteScalar());

            return count > 0;
        }

        // Compatibilidade com código antigo
        public string ObterPermissao(string email, string senha)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand(
                "SELECT permissao FROM view_decripto WHERE email=@e AND senha=@s LIMIT 1",
                conn);
            cmd.Parameters.AddWithValue("@e", email);
            cmd.Parameters.AddWithValue("@s", senha);

            var result = cmd.ExecuteScalar();
            return result == null || result == DBNull.Value ? string.Empty : Convert.ToString(result);
        }

        // Útil caso você queira só pelo email
        public string ObterPermissaoPorEmail(string email)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand(
                "SELECT permissao FROM view_decripto WHERE email=@e LIMIT 1",
                conn);
            cmd.Parameters.AddWithValue("@e", email);

            var result = cmd.ExecuteScalar();
            return result == null || result == DBNull.Value ? string.Empty : Convert.ToString(result);
        }
    }
}
