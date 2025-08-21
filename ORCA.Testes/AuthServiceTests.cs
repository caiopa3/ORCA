using MySql.Data.MySqlClient;
using ORCA.Services;
using System;
using NUnit.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ORCA.Testes
{
    [TestClass]
    public class AuthServiceTests
    {
        private AuthService _authService;

        [TestInitialize]
        public void Setup()
        {
            // Banco criado no container do GitHub Actions
            string servidor = "127.0.0.1"; 
            string bd = "banco"; 
            string usr = "root"; 
            string senha = "root";

            _authService = new AuthService(servidor, bd, usr, senha);
        }

        [TestMethod]
        public void TestLoginValido_Adm_DeveRetornarTrue()
        {
            bool result = _authService.ValidarLogin("yslan_adm@gmail.com", "123");
            Assert.IsTrue(result, "Login válido ADM deveria retornar true.");
        }

        [TestMethod]
        public void TestLoginValido_Usr_DeveRetornarTrue()
        {
            bool result = _authService.ValidarLogin("yslan_usr@gmail.com", "123");
            Assert.IsTrue(result, "Login válido USR deveria retornar true.");
        }

        [TestMethod]
        public void TestLoginValido_Ges_DeveRetornarTrue()
        {
            bool result = _authService.ValidarLogin("yslan_ges@gmail.com", "123");
            Assert.IsTrue(result, "Login válido GES deveria retornar true.");
        }

        [TestMethod]
        public void TestLoginInvalido_EmailErrado_DeveRetornarFalse()
        {
            bool result = _authService.ValidarLogin("email_invalido@gmail.com", "123");
            Assert.IsFalse(result, "Login com email inválido deveria retornar false.");
        }

        [TestMethod]
        public void TestLoginInvalido_SenhaErrada_DeveRetornarFalse()
        {
            bool result = _authService.ValidarLogin("yslan_adm@gmail.com", "senhaerrada");
            Assert.IsFalse(result, "Login com senha inválida deveria retornar false.");
        }
    }
}
