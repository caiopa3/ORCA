using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace ORCA.Services
{
    public class AuthService
    {
        private readonly string _connectionString;

        // VARIÁVEIS ANTI-BRUTEFORCE
        private static Dictionary<string, int> tentativas = new();
        private static Dictionary<string, DateTime> bloqueios = new();

        public AuthService(string servidor, string bd, string usr, string senha)
        {
            _connectionString = $"SERVER={servidor};PORT=3306;DATABASE={bd};UID={usr};PASSWORD={senha};";
        }

        // MÉTODO QUE VERIFICA SE O USUÁRIO ESTÁ BLOQUEADO
        private bool PodeTentarLogin(string email)
        {
            // Se existe bloqueio
            if (bloqueios.ContainsKey(email))
            {
                // Se o bloqueio AINDA não expirou
                if (DateTime.Now < bloqueios[email])
                    return false;

                // Se expirou → limpa bloqueio
                bloqueios.Remove(email);
                tentativas[email] = 0;
            }

            return true;
        }

        // MÉTODO QUE INCREMENTA TENTATIVAS
        private void RegistrarFalha(string email)
        {
            if (!tentativas.ContainsKey(email))
                tentativas[email] = 0;

            tentativas[email]++;

            // Se passar de 5 tentativas → BLOQUEIA POR 5 MINUTOS
            if (tentativas[email] >= 5)
            {
                bloqueios[email] = DateTime.Now.AddMinutes(5);
            }
        }

        // LOGIN MODIFICADO COM PROTEÇÃO
        public bool Login(string email, string senha)
        {
            // Bloqueado?
            if (!PodeTentarLogin(email))
                return false;

            // Login válido?
            if (ValidarLogin(email, senha))
            {
                tentativas[email] = 0; // zera tentativas ao logar
                return true;
            }
            else
            {
                RegistrarFalha(email);
                return false;
            }
        }

        // ----------------------------------------------------
        // 
        // ----------------------------------------------------

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
