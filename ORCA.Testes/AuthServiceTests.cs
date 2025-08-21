using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using ORCA.Services;
using System;
using NUnit.Framework;


namespace ORCA.Testes
{
    [TestFixture]
    public class AuthServiceTests
    {
        private AuthService _authService;

        [SetUp]
        public void Setup()
        {
            // Lê os dados de conexão do ambiente (definidos no workflow)
            string servidor = Environment.GetEnvironmentVariable("MYSQL_HOST") ?? "127.0.0.1";
            string bd = Environment.GetEnvironmentVariable("MYSQL_DATABASE") ?? "banco";
            string usr = Environment.GetEnvironmentVariable("MYSQL_USER") ?? "root";
            string senha = Environment.GetEnvironmentVariable("MYSQL_PASSWORD") ?? "root";

            _authService = new AuthService(servidor, bd, usr, senha);
        }

        [Test]
        public void TestLoginValido_Admin_DeveRetornarTrue()
        {
            bool resultado = _authService.ValidarLogin("yslan_adm@gmail.com", "123");
            Assert.IsTrue(resultado, "Login válido de administrador deveria retornar true.");
        }

        [Test]
        public void TestLoginValido_Usuario_DeveRetornarTrue()
        {
            bool resultado = _authService.ValidarLogin("yslan_usr@gmail.com", "123");
            Assert.IsTrue(resultado, "Login válido de usuário deveria retornar true.");
        }

        [Test]
        public void TestLoginValido_Gestor_DeveRetornarTrue()
        {
            bool resultado = _authService.ValidarLogin("yslan_ges@gmail.com", "123");
            Assert.IsTrue(resultado, "Login válido de gestor deveria retornar true.");
        }

        [Test]
        public void TestLoginInvalido_DeveRetornarFalse()
        {
            bool resultado = _authService.ValidarLogin("naoexiste@gmail.com", "senhaErrada");
            Assert.IsFalse(resultado, "Login inválido deveria retornar false.");
        }
    }
}
