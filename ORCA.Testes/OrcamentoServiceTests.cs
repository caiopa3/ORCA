using Microsoft.VisualStudio.TestTools.UnitTesting;
using ORCA.Services;
using System.Linq;
using MySql.Data.MySqlClient;

namespace ORCA.Testes
{
    [TestClass]
    public class OrcamentoServiceTests
    {
        private OrcamentoService _service;
        private string _emailTeste = "yslan_usr@gmail.com"; // já existe no banco
        private int _orcamentoCriadoId;

        [TestInitialize]
        public void Setup()
        {
            _service = new OrcamentoService("localhost", "banco", "root", "");
            _orcamentoCriadoId = 0;
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (_orcamentoCriadoId > 0)
            {
                using (var conn = new MySqlConnection("SERVER=localhost;DATABASE=banco;UID=root;PWD=;"))
                {
                    conn.Open();
                    var cmd = new MySqlCommand("DELETE FROM orcamento WHERE id=@id", conn);
                    cmd.Parameters.AddWithValue("@id", _orcamentoCriadoId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        [TestMethod]
        public void DeveInserirOrcamento()
        {
            var id = _service.InserirOrcamento("OrcamentoTeste_Insert", _emailTeste);
            _orcamentoCriadoId = id;
            Assert.IsTrue(id > 0, "Falha ao inserir orçamento.");
        }

        [TestMethod]
        public void DeveListarOrcamentos()
        {
            // insere primeiro
            var id = _service.InserirOrcamento("OrcamentoTeste_List", _emailTeste);
            _orcamentoCriadoId = id;

            var lista = _service.ListarPorEmail(_emailTeste);
            Assert.IsTrue(lista.Any(o => o.Id == id), "Orçamento inserido não foi encontrado na listagem.");
        }
    }
}
