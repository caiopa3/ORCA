using Microsoft.VisualStudio.TestTools.UnitTesting;
using ORCA.Services;
using MySql.Data.MySqlClient;
using System.Linq;

namespace ORCA.Testes
{
    [TestClass]
    public class OrcamentoServiceTests
    {
        private OrcamentoService _service;
        private string _connectionString = "SERVER=localhost;DATABASE=banco;UID=root;PASSWORD=;";
        private int _usuarioId;
        private string _emailTeste = "teste_orcamento@email.com";

        [TestInitialize]
        public void Setup()
        {
            _service = new OrcamentoService("localhost", "banco", "root", "");

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
        public void DeveInserirOrcamento()
        {
            int id = _service.InserirOrcamento("OrcamentoTeste", _emailTeste);
            Assert.IsTrue(id > 0);
        }

        [TestMethod]
        public void DeveListarOrcamentos()
        {
            _service.InserirOrcamento("OrcamentoTeste2", _emailTeste);

            var lista = _service.ListarPorEmail(_emailTeste);
            Assert.IsTrue(lista.Any());
        }
    }
}
