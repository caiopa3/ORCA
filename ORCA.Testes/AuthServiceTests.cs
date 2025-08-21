using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using ORCA.Services;

namespace ORCA.Testes
{
    [TestClass]
    public class AuthServiceTests
    {
        private string _connectionString = "Server=127.0.0.1;Port=3306;Database=banco;Uid=root;Pwd=root;";
        private AuthService _authService;

        [TestInitialize]
        public void Setup()
        {
            // recria a tabela de teste antes de cada execução
            using (var conexao = new MySqlConnection(_connectionString))
            {
                conexao.Open();

                string recreateTable = @"
                    DROP TABLE IF EXISTS usuario;
                    CREATE TABLE usuario (
                        id INT AUTO_INCREMENT PRIMARY KEY,
                        email TEXT,
                        senha VARCHAR(255),
                        permissao VARCHAR(3)
                    );";
                using (var cmd = new MySqlCommand(recreateTable, conexao))
                {
                    cmd.ExecuteNonQuery();
                }

                string insertData = @"
                    INSERT INTO usuario (email, senha, permissao) VALUES
                    ('yslan_adm@gmail.com', '123', 'adm'),
                    ('yslan_usr@gmail.com', '123', 'usr'),
                    ('yslan_ges@gmail.com', '123', 'ges');";
                using (var cmd = new MySqlCommand(insertData, conexao))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            _authService = new AuthService("127.0.0.1", "banco", "root", "root");
        }

        [TestMethod]
        public void TestLoginValido_DeveRetornarTrue()
        {
            bool resultado = _authService.ValidarLogin("yslan_adm@gmail.com", "123");
            Assert.IsTrue(resultado, "Login válido deveria retornar true.");
        }

        [TestMethod]
        public void TestLoginInvalido_DeveRetornarFalse()
        {
            bool resultado = _authService.ValidarLogin("email@errado.com", "senhaErrada");
            Assert.IsFalse(resultado, "Login inválido deveria retornar false.");
        }

        [TestMethod]
        public void TestPermissaoAdm_DeveRetornarAdm()
        {
            string permissao = _authService.ObterPermissao("yslan_adm@gmail.com", "123");
            Assert.AreEqual("adm", permissao, "Permissão esperada era 'adm'.");
        }

        [TestMethod]
        public void TestPermissaoUsr_DeveRetornarUsr()
        {
            string permissao = _authService.ObterPermissao("yslan_usr@gmail.com", "123");
            Assert.AreEqual("usr", permissao, "Permissão esperada era 'usr'.");
        }

        [TestMethod]
        public void TestPermissaoGes_DeveRetornarGes()
        {
            string permissao = _authService.ObterPermissao("yslan_ges@gmail.com", "123");
            Assert.AreEqual("ges", permissao, "Permissão esperada era 'ges'.");
        }
    }
}
