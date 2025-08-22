using Microsoft.VisualStudio.TestTools.UnitTesting;
using ORCA.Services;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace ORCA.Testes
{
    [TestClass]
    public class ModeloOrcamentoServiceTests
    {
        private ModeloOrcamentoService _service;
        private string _emailAdm = "yslan_adm@gmail.com"; // já existe no banco com id=1
        private int _modeloCriadoId;

        [TestInitialize]
        public void Setup()
        {
            _service = new ModeloOrcamentoService("localhost", "banco", "root", "");
            _modeloCriadoId = 0;
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (_modeloCriadoId > 0)
            {
                using (var conn = new MySqlConnection("SERVER=localhost;DATABASE=banco;UID=root;PWD=;"))
                {
                    conn.Open();

                    // remove associações de usuários
                    var cmd = new MySqlCommand("DELETE FROM modelo_orcamento_usuarios WHERE modelo_id=@id", conn);
                    cmd.Parameters.AddWithValue("@id", _modeloCriadoId);
                    cmd.ExecuteNonQuery();

                    // remove dados do modelo
                    cmd = new MySqlCommand("DELETE FROM modelo_orcamento_dados WHERE modelo_id=@id", conn);
                    cmd.Parameters.AddWithValue("@id", _modeloCriadoId);
                    cmd.ExecuteNonQuery();

                    // remove o modelo
                    cmd = new MySqlCommand("DELETE FROM modelo_orcamento WHERE id=@id", conn);
                    cmd.Parameters.AddWithValue("@id", _modeloCriadoId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        [TestMethod]
        public void DeveCriarModelo()
        {
            int usuarioId = _service.ObterUsuarioIdPorEmail(_emailAdm);
            Assert.IsTrue(usuarioId > 0, "Usuário de teste não encontrado no banco.");

            var colunas = new List<string> { "Produto", "Preço" };
            var linhas = new List<Dictionary<string, string>>
            {
                new() { ["Produto"] = "Notebook", ["Preço"] = "3500" }
            };

            int modeloId = _service.CriarModeloOrcamento(
                "ModeloTeste",
                usuarioId,
                colunas,
                linhas,
                new List<int>()
            );

            _modeloCriadoId = modeloId;

            Assert.IsTrue(modeloId > 0, "Modelo não foi criado corretamente.");
        }

        [TestMethod]
        public void DeveAssociarUsuarioAoModelo()
        {
            int usuarioId = _service.ObterUsuarioIdPorEmail(_emailAdm);
            Assert.IsTrue(usuarioId > 0, "Usuário de teste não encontrado no banco.");

            // cria um modelo primeiro
            int modeloId = _service.CriarModeloOrcamento(
                "ModeloAssociacao",
                usuarioId,
                new List<string> { "Coluna1" },
                new List<Dictionary<string, string>> { new() { ["Coluna1"] = "Teste" } },
                new List<int>()
            );
            _modeloCriadoId = modeloId;

            // associa o próprio usuário usando o método disponível no service
            _service.AdicionarUsuariosAoModelo(modeloId, new List<int> { usuarioId });

            // valida que associação existe
            using (var conn = new MySqlConnection("SERVER=localhost;DATABASE=banco;UID=root;PWD=;"))
            {
                conn.Open();
                var cmd = new MySqlCommand(
                    "SELECT COUNT(*) FROM modelo_orcamento_usuarios WHERE modelo_id=@modeloId AND usuario_id=@usuarioId",
                    conn
                );
                cmd.Parameters.AddWithValue("@modeloId", modeloId);
                cmd.Parameters.AddWithValue("@usuarioId", usuarioId);

                var count = (long)cmd.ExecuteScalar();
                Assert.IsTrue(count > 0, "Associação não foi criada corretamente.");
            }
        }

    }
}
