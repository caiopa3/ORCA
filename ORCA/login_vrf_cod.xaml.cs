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
        public login_vrf_cod(string emailUsuario)
        {
            InitializeComponent();
            email = emailUsuario;
        }

        private async Task<string> VerificarCodigoAsync(string email, string codigo)
        {
            string url = $"https://blanchedalmond-worm-516150.hostingersite.com/php/verificarCodigo.php?email={Uri.EscapeDataString(email)}&codigo={Uri.EscapeDataString(codigo)}";
            try
            {
                return await httpClient.GetStringAsync(url);
            }
            catch (HttpRequestException e)
            {
                return "Erro na requisição: " + e.Message;
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

            if (resposta == "OK")
            {
                MessageBox.Show("Código verificado com sucesso!");
                // Aqui você pode prosseguir com o fluxo da aplicação
                this.Close();
            }
            else
            {
                MessageBox.Show("Código incorreto ou expirado.");
            }
        }
    }
}
