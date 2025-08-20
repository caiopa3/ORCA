using Microsoft.VisualStudio.TestTools.UnitTesting;
using ORCA.Services;

namespace ORCA.Testes
{
    [TestClass]
public class AuthServiceTests
{
    private string _connectionString;

    [TestInitialize]
    public void Setup()
    {
        _connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
                          ?? "server=127.0.0.1;port=3306;database=orca;uid=root;pwd=root;";

        using (var conexao = new MySqlConnection(_connectionString))
        {
            conexao.Open();

            // Cria a tabela
            string createTable = @"
                CREATE TABLE IF NOT EXISTS usuario (
                    id INT AUTO_INCREMENT PRIMARY KEY,
                    email TEXT,
                    senha VARCHAR(255),
                    permissao VARCHAR(3)
                );";
            new MySqlCommand(createTable, conexao).ExecuteNonQuery();

            // Limpa tabela
            new MySqlCommand("DELETE FROM usuario;", conexao).ExecuteNonQuery();

            // Insere seus fakes
            string insert = @"
                INSERT INTO usuario (email, senha, permissao) VALUES
                ('yslan_adm@gmail.com', '123', 'adm'),
                ('yslan_usr@gmail.com', '123', 'usr'),
                ('yslan_ges@gmail.com', '123', 'ges');";
            new MySqlCommand(insert, conexao).ExecuteNonQuery();
        }
    }

    [TestMethod]
    public void TestLoginValido_DeveRetornarTrue()
    {
        var auth = new AuthService("localhost", "orca", "root", "root");
        bool result = auth.ValidarLogin("yslan_adm@gmail.com", "123");
        Assert.IsTrue(result, "Login válido deveria retornar true.");
    }

    [TestMethod]
    public void TestLoginInvalido_DeveRetornarFalse()
    {
        var auth = new AuthService("localhost", "orca", "root", "root");
        bool result = auth.ValidarLogin("fake@fake.com", "errado");
        Assert.IsFalse(result, "Login inválido deveria retornar false.");
    }

    [TestMethod]
    public void TestObterPermissao_Adm()
    {
        var auth = new AuthService("localhost", "orca", "root", "root");
        string permissao = auth.ObterPermissao("yslan_adm@gmail.com", "123");
        Assert.AreEqual("adm", permissao);
    }
}

}
