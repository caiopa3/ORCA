using System;
using System.Collections.Generic;
using System.Data;
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
using static ORCA.Services.OrcamentoService;

namespace ORCA
{
    /// <summary>
    /// Lógica interna para altFunc_adm.xaml
    /// </summary>
    public partial class altFunc_adm : Window
    {
        private readonly Services.OrcamentoService service;
        private readonly string emailUsuario;


        public altFunc_adm(string email, Services.OrcamentoService srv)
        {
            InitializeComponent();

            emailUsuario = email;
            service = srv;

            // Preenche a ComboBox de permissões
            CarregarPermissoes();

            // Carrega dados do usuário
            CarregarUsuario();

        }
        private void CarregarPermissoes()
        {
            try
            {
                var permissoesDoBanco = service.ListarPermissoes();

                // transforma para exibição amigável
                var listaExibicao = permissoesDoBanco.Select(p => new PermissaoItem
                {
                    Valor = p,
                    Texto = p == "adm" ? "Administrador" :
                            p == "usr" ? "Usuário" :
                            p == "ges" ? "Gestor" :
                            p // qualquer outro valor
                }).ToList();

                cmbPermissao.ItemsSource = listaExibicao;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar permissões: " + ex.Message);
            }
        }

        private void CarregarUsuario()
        {
            DataRow row = service.BuscarUsuarioPorEmail(emailUsuario);
            if (row != null)
            {
                txtNomeCompleto.Text = row["nome_completo"].ToString();
                txtEmail.Text = row["email"].ToString();
                txtTelefone.Text = row["telefone_celular"].ToString();
                txtCPF.Text = row["cpf"].ToString();
                txtRG.Text = row["rg"].ToString();
                cmbPermissao.SelectedItem = row["permissao"].ToString();
            }
        }

        private void btnSalvar_Click(object sender, RoutedEventArgs e)
        {
            string novaPermissao = cmbPermissao.SelectedItem?.ToString() ?? "usr";
            string novoNome = txtNomeCompleto.Text.Trim();
            string telefone = txtTelefone.Text.Trim();
            string cpf = txtCPF.Text.Trim();
            string rg = txtRG.Text.Trim();

            // Atualiza dados do usuário
            service.AtualizarUsuarioCompleto(emailUsuario, novoNome, telefone, cpf, rg, novaPermissao);

            MessageBox.Show("Usuário atualizado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

    }
}
