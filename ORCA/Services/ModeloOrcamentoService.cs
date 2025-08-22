using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ORCA.Services
{
    public class ModeloOrcamentoService
    {
        private readonly string _connectionString;

        public ModeloOrcamentoService(string servidor, string bd, string usr, string senha)
        {
            _connectionString = $"SERVER={servidor};PORT=3306;DATABASE={bd};UID={usr};PASSWORD={senha};";
        }

        public int ObterUsuarioIdPorEmail(string email)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand("SELECT id FROM usuario WHERE email=@e LIMIT 1", conn);
            cmd.Parameters.AddWithValue("@e", email);
            var result = cmd.ExecuteScalar();

            if (result == null || result == DBNull.Value)
                throw new InvalidOperationException("Usuário não encontrado para o email informado.");

            return Convert.ToInt32(result);
        }

        public int CriarModeloOrcamento(
            string nomeModelo,
            int usuarioCriadorId,
            List<string> colunas,
            List<Dictionary<string, object>> linhas,
            List<int> usuariosCompartilhados)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            // 1) cria modelo
            var cmdModelo = new MySqlCommand(
                "INSERT INTO modelo_orcamento (nome, usr_criador_id) VALUES (@n, @u)",
                conn);
            cmdModelo.Parameters.AddWithValue("@n", nomeModelo);
            cmdModelo.Parameters.AddWithValue("@u", usuarioCriadorId);
            cmdModelo.ExecuteNonQuery();
            int modeloId = (int)cmdModelo.LastInsertedId;

            // 2) salva JSON
            var pacote = new { Colunas = colunas, Linhas = linhas };
            string json = JsonConvert.SerializeObject(pacote);

            var cmdDados = new MySqlCommand(
                "INSERT INTO modelo_orcamento_dados (modelo_id, dados_json) VALUES (@m, @j)",
                conn);
            cmdDados.Parameters.AddWithValue("@m", modeloId);
            cmdDados.Parameters.AddWithValue("@j", json);
            cmdDados.ExecuteNonQuery();

            // 3) compartilhamento (opcional)
            foreach (var uid in usuariosCompartilhados)
            {
                var cmdPerm = new MySqlCommand(
                    "INSERT INTO modelo_orcamento_usuarios (modelo_id, usuario_id) VALUES (@m, @u)",
                    conn);
                cmdPerm.Parameters.AddWithValue("@m", modeloId);
                cmdPerm.Parameters.AddWithValue("@u", uid);
                cmdPerm.ExecuteNonQuery();
            }

            return modeloId;
        }
    }
}
