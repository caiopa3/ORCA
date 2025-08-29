using ORCA.Services;
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
using static ORCA.Services.OrcamentoService;

namespace ORCA
{
    /// <summary>
    /// Lógica interna para cadFunc_adm.xaml
    /// </summary>
    public partial class cadFunc_adm : Window
    {

        private OrcamentoService service;
        public string servidor = "";
        public string bd = "";
        public string usr = "";
        public string senha_usuario = "";
        public string email_usuario = "";

        public cadFunc_adm(string e, string s, string b, string u, string se)
        {
            InitializeComponent();
            email_usuario = e;
            servidor = s;
            bd = b;
            usr = u;
            senha_usuario = se;

            // cria o service já com a conexão
            service = new OrcamentoService("localhost", "banco", "root", "");

            CarregarPermissoes();
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
                            p // qualquer outro valor
                }).ToList();

                cmbPermissao.ItemsSource = listaExibicao;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar permissões: " + ex.Message);
            }
        }

        private void btn_cad_func_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string email = txtEmail.Text.Trim();
                string senha = txtSenha.Text.Trim();
                string permissao = (cmbPermissao.SelectedItem as PermissaoItem)?.Valor;

                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha) || string.IsNullOrWhiteSpace(permissao))
                {
                    MessageBox.Show("Preencha todos os campos!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                service.CadastrarUsuario(email, senha, permissao);

                MessageBox.Show("Usuário cadastrado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                gereFunc_adm gereFunc_Adm = new gereFunc_adm(email_usuario, servidor, bd, usr, senha_usuario);
                gereFunc_Adm.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao cadastrar usuário: " + ex.Message);
            }
        }
    }
}
