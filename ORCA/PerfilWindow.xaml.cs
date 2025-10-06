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
                string novoEmail = txtEmail.Text.Trim();
                string novaSenha = txtSenha.Password.Trim();

                if (string.IsNullOrWhiteSpace(novoEmail) || string.IsNullOrWhiteSpace(novaSenha))
                {
                    MessageBox.Show("Preencha todos os campos.");
                    return;
                }

                _orcamentoService.AtualizarUsuario(_emailAtual, novoEmail, novaSenha);

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
