using System;
using System.Windows;
using ORCA.Services;

namespace ORCA
{
    /// <summary>
    /// Lógica interna para login_alt_senha.xaml
    /// </summary>
    public partial class login_alt_senha : Window
    {
        private readonly OrcamentoService _orcamentoService;
        private readonly string _emailAtual;

        public login_alt_senha(string email, string servidor, string bd, string usr, string senha)
        {
            InitializeComponent();
            _orcamentoService = new OrcamentoService(servidor, bd, usr, senha);
            _emailAtual = email;
        }

        private void btnVerificar_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                string novaSenha = txtSenha.Password.Trim();

                if (string.IsNullOrWhiteSpace(novaSenha))
                {
                    MessageBox.Show("Digite a nova senha.");
                    return;
                }

                // Atualiza a senha, mantendo o mesmo email
                _orcamentoService.AtualizarUsuario(_emailAtual, _emailAtual, novaSenha);

                MessageBox.Show("Senha alterada com sucesso!");

                // Volta para a tela de login
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao atualizar senha: " + ex.Message);
            }
        }
    }
}
