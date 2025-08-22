using Microsoft.VisualStudio.TestTools.UnitTesting;
using ORCA.Services;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace ORCA.Testes
{
    [TestClass]
    public class ModeloOrcamentoServiceTests
    {
        private ModeloOrcamentoService _service;
        private string _connectionString = "SERVER=localhost;DATABASE=banco;UID=root;PASSWORD=;";
        private int _usuarioId;
        private string _emailTeste = "teste_modelo@email.com";

        [TestInitialize]
        public void Setup()
        {
            _service = new ModeloOrcamentoService("localhost", "banco", "root", "");

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand(
                "INSERT INTO usuario (email, senha, permissao) VALUES (@Email, '123', 'usr')", conn);
            cmd.Parameters.AddWithValue("@Email", _emailTeste);
            cmd.ExecuteNonQuery();

            _usuarioId = (int)cmd.LastInsertedId;
        }

        [TestCleanup]
        public void Cleanup()
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand("DELETE FROM usuario WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", _usuarioId);
            cmd.ExecuteNonQuery();
        }

        [TestMethod]
        public void DeveCriarModelo()
        {
            var colunas = new List<string> { "Coluna1", "Coluna2" };
            var linhas = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object> { { "Coluna1", "valor1" }, { "Coluna2", "valor2" } }
            };

            int id = _service.CriarModeloOrcamento("ModeloTeste", _usuarioId, colunas, linhas, new List<int>());
            Assert.IsTrue(id > 0);
        }
    }
}
