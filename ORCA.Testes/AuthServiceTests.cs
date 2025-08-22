using Microsoft.VisualStudio.TestTools.UnitTesting;
using ORCA.Services;

namespace ORCA.Testes
{
    [TestClass]
    public class AuthServiceTests
    {
        private AuthService _service;

        [TestInitialize]
        public void Setup()
        {
            _service = new AuthService("localhost", "banco", "root", "");
        }

        [TestMethod]
        public void DeveLogarComUsuarioValido()
        {
            var ok = _service.Login("yslan_usr@gmail.com", "123");
            Assert.IsTrue(ok);
        }

        [TestMethod]
        public void NaoDeveLogarComSenhaErrada()
        {
            var ok = _service.Login("yslan_usr@gmail.com", "senhaErrada");
            Assert.IsFalse(ok);
        }

        [TestMethod]
        public void NaoDeveLogarComUsuarioInexistente()
        {
            var ok = _service.Login("naoexiste@email.com", "123");
            Assert.IsFalse(ok);
        }

        [TestMethod]
        public void NaoDeveLogarComCamposVazios()
        {
            var ok = _service.Login("", "");
            Assert.IsFalse(ok);
        }
    }
}
