using Microsoft.VisualStudio.TestTools.UnitTesting;
using ORCA.Services;

namespace ORCA.Testes
{
    [TestClass]
    public class AuthServiceTests
    {
        private AuthService _authService;

        [TestInitialize]
        public void Setup()
        {
            // use os mesmos dados do ORCA/ORCA.csproj
            string servidor = "localhost";
            string bd = "banco";
            string usr = "root";
            string senha = "";

            _authService = new AuthService(servidor, bd, usr, senha);
        }

        [TestMethod]
        public void TestLoginValido_DeveRetornarTrue()
        {
            // arrange
            string email = "yslan_usr@gmail.com";  // precisa existir no banco
            string password = "123";           // senha que você cadastrou no banco

            // act
            bool resultado = _authService.ValidarLogin(email, password);

            // assert
            Assert.IsTrue(resultado, "Login válido deveria retornar true.");
        }

        [TestMethod]
        public void TestLoginInvalido_DeveRetornarFalse()
        {
            string email = "naoexiste@gmail.com";
            string password = "senhaerrada";

            bool resultado = _authService.ValidarLogin(email, password);

            Assert.IsFalse(resultado, "Login inválido deveria retornar false.");
        }

        [TestMethod]
        public void TestObterPermissao_UsuarioUsr()
        {
            string email = "yslan_usr@gmail.com"; // no banco, este usuário deve ter permissao = 'usr'
            string password = "123";

            string permissao = _authService.ObterPermissao(email, password);

            Assert.AreEqual("usr", permissao, "Permissão esperada era 'usr'.");
        }

        [TestMethod]
        public void TestObterPermissao_UsuarioAdm()
        {
            string email = "yslan_adm@gmail.com"; // no banco, este deve ter permissao = 'adm'
            string password = "123";

            string permissao = _authService.ObterPermissao(email, password);

            Assert.AreEqual("adm", permissao, "Permissão esperada era 'adm'.");
        }

        [TestMethod]
        public void TestObterPermissao_UsuarioGes()
        {
            string email = "yslan_ges@gmail.com"; // no banco, este deve ter permissao = 'ges'
            string password = "123";

            string permissao = _authService.ObterPermissao(email, password);

            Assert.AreEqual("ges", permissao, "Permissão esperada era 'ges'.");
        }
    }
}
