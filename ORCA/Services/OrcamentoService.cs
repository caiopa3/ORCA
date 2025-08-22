using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

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
    }
}
