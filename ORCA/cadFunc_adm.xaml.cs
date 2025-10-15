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

        private void btn_cad_func_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Coleta campos
                var nomeCompleto = txtNomeCompleto.Text.Trim();

                var email = txtEmail.Text.Trim();
                var senhaPlain = txtSenha.Password;
                var telCelular = txtTelefone.Text.Trim();
                var permissao = (cmbPermissao.SelectedItem as OrcamentoService.PermissaoItem)?.Valor;
                var cpf = txtCPF.Text.Trim();
                var rg = txtRG.Text.Trim();

                // Validação

                if (string.IsNullOrWhiteSpace(nomeCompleto))
                {
                    MessageBox.Show("O nome completo é obrigatório.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(email))
                {
                    MessageBox.Show("O e-mail é obrigatório.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(senhaPlain))
                {
                    MessageBox.Show("A senha é obrigatória.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(telCelular))
                {
                    MessageBox.Show("O telefone é obrigatório.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(cpf))
                {
                    MessageBox.Show("O CPF é obrigatório.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(rg))
                {
                    MessageBox.Show("O RG é obrigatório.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (cmbPermissao.SelectedItem == null)
                {
                    MessageBox.Show("Selecione uma permissão.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Hash da senha (recomendado)
                string senhaParaSalvar;
                try
                {
                    // Recomendado: instalar BCrypt.Net-Next via NuGet e descomentar abaixo:
                    // senhaParaSalvar = BCrypt.Net.BCrypt.HashPassword(senhaPlain);

                    // Se não usar BCrypt, você deve substituir por sua rotina segura.
                    senhaParaSalvar = senhaPlain; // ATENÇÃO: se manter, será salvo em texto plano — não usar em produção.
                }
                catch
                {
                    senhaParaSalvar = senhaPlain;
                }

                // Chama service
                service.CadastrarUsuarioCompleto(
                    nomeCompleto,
                    email,
                    senhaParaSalvar,
                    telCelular,                   
                    permissao, 
                    cpf,
                    rg     
                );

                MessageBox.Show("Usuário cadastrado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                // volta para a tela de gerenciamento (se existir)
                var gereFunc_Adm = new gereFunc_adm(email_usuario, servidor, bd, usr, senha_usuario);
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
