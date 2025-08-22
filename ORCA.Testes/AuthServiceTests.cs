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
    }
}
