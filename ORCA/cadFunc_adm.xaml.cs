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
                var nomeSocial = txtNomeSocial.Text.Trim();
                var dataNascimento = dpDataNascimento.Text;
                var email = txtEmail.Text.Trim();
                var senhaPlain = txtSenha.Password;
                var telCelular = txtTelCelular.Text.Trim();
                var telFixo = txtTelFixo.Text.Trim();

                var logradouro = txtLogradouro.Text.Trim();
                var numero = txtNumero.Text.Trim();
                var complemento = txtComplemento.Text.Trim();
                var bairro = txtBairro.Text.Trim();
                var cidade = txtCidade.Text.Trim();
                var uf = (cmbUF.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? (cmbUF.SelectedItem?.ToString() ?? "");
                var cep = txtCEP.Text.Trim();

                var permissao = (cmbPermissao.SelectedItem as OrcamentoService.PermissaoItem)?.Valor;
                var cargo = txtCargo.Text.Trim();
                var departamento = txtDepartamento.Text.Trim();
                var dataAdmissao = dpDataAdmissao.Text;
                var tipoContrato = (cmbTipoContrato.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
                var regimeJornada = (cmbRegimeJornada.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
                var salarioBase = txtSalarioBase.Text.Trim();

                var cpf = txtCPF.Text.Trim();
                var rg = txtRG.Text.Trim();
                var orgaoExpedidor = txtOrgaoExpedidor.Text.Trim();
                var dataExpedicao = dpDataExpedicao.Text;

                var banco = txtBanco.Text.Trim();
                var agencia = txtAgencia.Text.Trim();
                var conta = txtConta.Text.Trim();

                var infoMedicas = txtInfoMedicas.Text.Trim();
                var contatoEmergNome = txtContatoEmergNome.Text.Trim();
                var contatoEmergTel = txtContatoEmergTel.Text.Trim();
                var relacaoContato = txtRelacaoContato.Text.Trim();

                // validações básicas
                if (string.IsNullOrWhiteSpace(nomeCompleto) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senhaPlain) || string.IsNullOrWhiteSpace(permissao))
                {
                    MessageBox.Show("Preencha pelo menos: nome completo, email, senha e permissões.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                    nomeSocial,
                    dataNascimento,
                    email,
                    senhaParaSalvar,
                    telCelular,
                    telFixo,
                    logradouro,
                    numero,
                    complemento,
                    bairro,
                    cidade,
                    uf,
                    cep,
                    permissao,
                    cargo,
                    departamento,
                    dataAdmissao,
                    tipoContrato,
                    regimeJornada,
                    salarioBase,
                    cpf,
                    rg,
                    orgaoExpedidor,
                    dataExpedicao,
                    banco,
                    agencia,
                    conta,
                    infoMedicas,
                    contatoEmergNome,
                    contatoEmergTel,
                    relacaoContato
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
