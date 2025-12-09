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
    /// Lógica interna para login_esq_senha.xaml
    /// </summary>
    public partial class login_esq_senha : Window
    {
        public string servidor = "";
        public string bd = "";
        public string usr = "";
        public string senha = "";

        private readonly HttpClient httpClient = new HttpClient();
        public login_esq_senha(string s, string b, string u, string se)
        {
            InitializeComponent();

            servidor = s;
            bd = b;
            usr = u;
            senha = se;

        }
        private async Task<string> EnviarCodigoPorEmailAsync(string email)
        {
            string url = $"https://fecceteceuroalbinodesouza.com.br/enviarEmail.php?email={Uri.EscapeDataString(email)}";

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


        private async void btnEnviar_Click_1(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text.Trim();

            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Por favor, insira um email válido.");
                return;
            }

            string resposta = await EnviarCodigoPorEmailAsync(email);

            MessageBox.Show(resposta);

            if (resposta.Contains("Sucesso"))
            {
                // Abrir a janela de verificação de código, passando o email
                login_vrf_cod login_vrf_cod = new login_vrf_cod(email, servidor, bd, usr, senha);
                login_vrf_cod.Show();

                // Fecha esta janela
                this.Close();
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
