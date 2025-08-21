using NUnit.Framework;
using ORCA.Services;

namespace ORCA.Testes
{
    [TestFixture]
    public class AuthServiceTests
    {
        private AuthService _authService;

        [SetUp]
        public void Setup()
        {
            // Banco configurado no GitHub Actions (mysql service)
            string servidor = "127.0.0.1";
            string bd = "banco";
            string usr = "root";
            string senha = "root";

            _authService = new AuthService(servidor, bd, usr, senha);
        }

        [Test]
        public void TestLoginValido_DeveRetornarTrue()
        {
            // Este usuário foi inserido pelo workflow (job Criar tabela e inserir dados fake)
            bool result = _authService.ValidarLogin("yslan_adm@gmail.com", "123");
            Assert.IsTrue(result, "Login válido deveria retornar true.");
        }

        [Test]
        public void TestLoginInvalido_DeveRetornarFalse()
        {
            bool result = _authService.ValidarLogin("email_invalido@gmail.com", "senha_errada");
            Assert.IsFalse(result, "Login inválido deveria retornar false.");
        }

        [Test]
        public void TestPermissao_DeveRetornarAdm()
        {
            string permissao = _authService.ObterPermissao("yslan_adm@gmail.com", "123");
            Assert.AreEqual("adm", permissao, "A permissão deveria ser 'adm'.");
        }
    }
}
