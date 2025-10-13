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
                string senhaAtual = txtSenhaAtual.Password.Trim();
                string novaSenha = txtNovaSenha.Password.Trim();
                string confirmarSenha = txtConfirmarSenha.Password.Trim();

                bool atualizarEmail = chkAtualizarEmail.IsChecked == true;
                string novoEmail = txtNovoEmail.Text.Trim();
                string confirmarEmail = txtConfirmarEmail.Text.Trim();

                // Validação básica
                if (string.IsNullOrWhiteSpace(senhaAtual) ||
                    string.IsNullOrWhiteSpace(novaSenha) ||
                    string.IsNullOrWhiteSpace(confirmarSenha))
                {
                    MessageBox.Show("Preencha todos os campos de senha.");
                    return;
                }

                if (novaSenha != confirmarSenha)
                {
                    MessageBox.Show("As senhas novas não coincidem.");
                    return;
                }

                // Se for atualizar e-mail
                if (atualizarEmail)
                {
                    if (string.IsNullOrWhiteSpace(novoEmail) || string.IsNullOrWhiteSpace(confirmarEmail))
                    {
                        MessageBox.Show("Preencha os campos de e-mail.");
                        return;
                    }

                    if (txtEmail.Text != confirmarEmail)
                    {
                        MessageBox.Show("Os e-mails não coincidem.");
                        return;
                    }
                }

                // Verifica senha atual
                bool senhaCorreta = _orcamentoService.VerificarSenhaAtual(_emailAtual, senhaAtual);
                if (!senhaCorreta)
                {
                    MessageBox.Show("A senha atual está incorreta.");
                    return;
                }

                // Atualiza dados no banco
                if (atualizarEmail)
                {
                    _orcamentoService.AtualizarUsuario(_emailAtual, novoEmail, novaSenha);
                    Sessao.email = novoEmail; // Atualiza na sessão
                }
                else
                {
                    _orcamentoService.AtualizarUsuario(_emailAtual, _emailAtual, novaSenha);
                }

                MessageBox.Show("Dados atualizados com sucesso!");

                this.DialogResult = true; // fecha janela com sucesso
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao atualizar: " + ex.Message);
            }
        }

        private void btnVoltar_Click(object sender, RoutedEventArgs e)
        {

            this.DialogResult = false; // Apenas fecha a janela de perfil

        }

        private void chkAtualizarEmail_Checked(object sender, RoutedEventArgs e)
        {
            panelNovoEmail.Visibility = Visibility.Visible;
        }

        private void chkAtualizarEmail_Unchecked(object sender, RoutedEventArgs e)
        {
            panelNovoEmail.Visibility = Visibility.Collapsed;
        }
    }
}
