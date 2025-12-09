using System;
using System.Text.RegularExpressions;
using System.Windows;
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

                // Validação básica de senhas
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

                // Se for atualizar e-mail, valida novo email e confirmação
                if (atualizarEmail)
                {
                    if (string.IsNullOrWhiteSpace(novoEmail) || string.IsNullOrWhiteSpace(confirmarEmail))
                    {
                        MessageBox.Show("Preencha os campos de novo e-mail e confirmação.");
                        return;
                    }

                    // compara o novoEmail com a confirmação (case-insensitive)
                    if (!string.Equals(novoEmail, confirmarEmail, StringComparison.OrdinalIgnoreCase))
                    {
                        MessageBox.Show("Os e-mails não coincidem.");
                        return;
                    }

                    if (!IsValidEmail(novoEmail))
                    {
                        MessageBox.Show("Novo e-mail inválido.");
                        return;
                    }
                }

                // Verifica senha atual (faça isso depois das validações acima)
                bool senhaCorreta = _orcamentoService.VerificarSenhaAtual(_emailAtual, senhaAtual);
                if (!senhaCorreta)
                {
                    MessageBox.Show("A senha atual está incorreta.");
                    return;
                }

                // Determina qual e-mail vai ser gravado (se não trocar, grava o mesmo)
                string emailAlvo = atualizarEmail ? novoEmail : _emailAtual;

                // Atualiza dados no banco
                _orcamentoService.AtualizarUsuario(_emailAtual, emailAlvo, novaSenha);

                // Atualiza sessão / UI se trocou de e-mail
                if (atualizarEmail)
                {
                    Sessao.email = novoEmail; // atualiza sessão global (se você usa isso)
                    txtEmail.Text = novoEmail; // mostra novo e-mail na tela
                    // Observação: _emailAtual é readonly — se outras partes do código
                    // dependem do campo _emailAtual nesta instância, você pode pedir ao usuário
                    // para reabrir a janela ou fazer logout/login para sincronizar por completo.
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

        // Validação simples de formato de email
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            try
            {
                // Regex simples e eficiente para validação básica (não tenta validar TLDs estranhos)
                var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }
    }
}
