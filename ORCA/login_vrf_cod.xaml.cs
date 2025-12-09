using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

namespace ORCA
{
    /// <summary>
    /// Lógica interna para login_vrf_cod.xaml
    /// </summary>
    public partial class login_vrf_cod : Window
    {
        private readonly HttpClient httpClient = new HttpClient();
        private readonly string email;

        public string servidor = "";
        public string bd = "";
        public string usr = "";
        public string senha = "";

        public login_vrf_cod(string emailUsuario, string s, string b, string u, string se)
        {
            InitializeComponent();

            email = emailUsuario;
            servidor = s;
            bd = b;
            usr = u;
            senha = se;

        }

        private async Task<string> VerificarCodigoAsync(string email, string codigo)
        {
            string url = $"https://fecceteceuroalbinodesouza.com.br/verificarCodigo.php?email={Uri.EscapeDataString(email)}&codigo={Uri.EscapeDataString(codigo)}";

            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(15);
                    string resposta = await httpClient.GetStringAsync(url);
                    return resposta.Trim();
                }
            }
            catch (HttpRequestException e)
            {
                return $"Erro ao conectar com o servidor: {e.Message}";
            }
            catch (TaskCanceledException)
            {
                return "Erro: o servidor demorou para responder (timeout).";
            }
            catch (Exception ex)
            {
                return $"Erro inesperado: {ex.Message}";
            }
        }


        private async void btnVerificar_Click_1(object sender, RoutedEventArgs e)
        {
            string codigo = txtCodigo.Text.Trim();

            if (string.IsNullOrEmpty(codigo))
            {
                MessageBox.Show("Por favor, insira o código recebido.");
                return;
            }

            string resposta = await VerificarCodigoAsync(email, codigo);

            if (resposta == "Ok")
            {
                MessageBox.Show("Código verificado com sucesso!");
                // Aqui você pode prosseguir com o fluxo da aplicação
                login_alt_senha login_alt_senha = new login_alt_senha(email, servidor, bd, usr, senha);
                login_alt_senha.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Código incorreto ou expirado.");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow telaInicial = new MainWindow();
            telaInicial.Show();
            this.Close();
        }
    }
}
