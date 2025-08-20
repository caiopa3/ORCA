using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using ORCA.Services;
using System;

namespace ORCA.Testes
{
    [TestClass]
    public class AuthServiceTests
    {
        private string _connectionString;
        private AuthService _authService;

        [TestInitialize]
        public void Setup()
        {
            // Pega a connection string do ambiente (GitHub Actions define no workflow)
            _connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
                               ?? "server=127.0.0.1;port=3306;database=orca;uid=root;pwd=root;";

            _authService = new AuthService("127.0.0.1", "orca", "root", "root");

            using (var conexao = new MySqlConnection(_connectionString))
            {
                conexao.Open();

                // Cria tabela usuario (caso não exista)
                var createTable = @"
                CREATE TABLE IF NOT EXISTS usuario (
                    id INT AUTO_INCREMENT PRIMARY KEY,
                    email TEXT,
                    senha VARCHAR(255),
                    permissao VARCHAR(3)
                );";

                using (var cmd = new MySqlCommand(createTable, conexao))
                {
                    cmd.ExecuteNonQuery();
                }

                // Limpa dados antigos
                using (var cmd = new MySqlCommand("DELETE FROM usuario;", conexao))
                {
                    cmd.ExecuteNonQuery();
                }

                // Insere dados fake
                var insertFake = @"
                INSERT INTO usuario (email, senha, permissao) VALUES
                ('yslan_adm@gmail.com', '123', 'adm'),
                ('yslan_usr@gmail.com', '123', 'usr'),
                ('yslan_ges@gmail.com', '123', 'ges');";

                using (var cmd = new MySqlCommand(insertFake, conexao))
                {
                    cmd.ExecuteNonQuery();
                }
            }
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
            bool resultado = _authService.ValidarLogin("naoexiste@gmail.com", "abc");
            Assert.IsFalse(resultado, "Login inválido deveria retornar false.");
        }

        [TestMethod]
        public void TestLoginUsr_DeveRetornarTrue()
        {
            bool resultado = _authService.ValidarLogin("yslan_usr@gmail.com", "123");
            Assert.IsTrue(resultado, "Login de usuário usr deveria retornar true.");
        }

        [TestMethod]
        public void TestLoginAdm_DeveRetornarTrue()
        {
            bool resultado = _authService.ValidarLogin("yslan_adm@gmail.com", "123");
            Assert.IsTrue(resultado, "Login de usuário adm deveria retornar true.");
        }

        [TestMethod]
        public void TestLoginGes_DeveRetornarTrue()
        {
            bool resultado = _authService.ValidarLogin("yslan_ges@gmail.com", "123");
            Assert.IsTrue(resultado, "Login de usuário ges deveria retornar true.");
        }
    }
}
