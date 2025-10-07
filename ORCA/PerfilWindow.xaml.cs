using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ORCA.Services;

namespace ORCA
{
    /// <summary>
    /// Lógica interna para PerfilWindow.xaml
    /// </summary>
    public partial class PerfilWindow : Window
    {
        private readonly OrcamentoService _orcamentoService;
        private readonly string _emailAtual;

        public PerfilWindow(string email, OrcamentoService service)
        {
            InitializeComponent();
            _orcamentoService = service;
            _emailAtual = email;
            txtEmail.Text = email;
        }

        private void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string email = txtEmail.Text.Trim();
                string confirmarEmail = txtConfirmarEmail.Text.Trim();
                string senhaAtual = txtSenhaAtual.Password.Trim();
                string novaSenha = txtNovaSenha.Password.Trim();
                string confirmarSenha = txtConfirmarSenha.Password.Trim();

                // Validações
                if (string.IsNullOrWhiteSpace(email) ||
                    string.IsNullOrWhiteSpace(confirmarEmail) ||
                    string.IsNullOrWhiteSpace(senhaAtual) ||
                    string.IsNullOrWhiteSpace(novaSenha) ||
                    string.IsNullOrWhiteSpace(confirmarSenha))
                {
                    MessageBox.Show("Preencha todos os campos.");
                    return;
                }

                if (email != confirmarEmail)
                {
                    MessageBox.Show("Os e-mails não coincidem.");
                    return;
                }

                if (novaSenha != confirmarSenha)
                {
                    MessageBox.Show("As senhas novas não coincidem.");
                    return;
                }

                // Verifica senha atual no banco
                bool senhaCorreta = _orcamentoService.VerificarSenhaAtual(_emailAtual, senhaAtual);
                if (!senhaCorreta)
                {
                    MessageBox.Show("A senha atual está incorreta.");
                    return;
                }

                // Atualiza
                _orcamentoService.AtualizarUsuario(_emailAtual, email, novaSenha);

                MessageBox.Show("Dados atualizados com sucesso!");
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao atualizar: " + ex.Message);
            }
        }

    }
}
