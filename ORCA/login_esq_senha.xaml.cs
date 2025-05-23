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
        private readonly HttpClient httpClient = new HttpClient();
        public login_esq_senha()
        {
            InitializeComponent();
        }
        private async Task<string> EnviarCodigoPorEmailAsync(string email)
        {
            string url = $"https://blanchedalmond-worm-516150.hostingersite.com/php/enviarEmail.php?email={Uri.EscapeDataString(email)}";
            try
            {
                return await httpClient.GetStringAsync(url);
            }
            catch (HttpRequestException e)
            {
                return "Erro na requisição: " + e.Message;
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

            if (resposta.Contains("sucesso"))
            {
                // Abrir a janela de verificação de código, passando o email
                login_vrf_cod login_vrf_cod = new login_vrf_cod(email);
                login_vrf_cod.Show();

                // Fecha esta janela
                this.Close();
            }
        }
    }
}
