using System.Text;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Security.Cryptography;
using ORCA.Services;
using System.Windows.Input;
using System.Windows.Threading;

namespace ORCA
{
    public partial class MainWindow : Window
    {
        public string servidor = "localhost";
        public string bd = "banco";
        public string usr = "root";
        public string senha = "";
        public string connectionString;

        private const string RepoOwner = "caiopa3";
        private const string RepoName = "ORCA";
        private const string CurrentVersion = "1.0.2";

        // 🔥 Tentativas (front-end)
        int maxTentativas = 5;
        int tentativasRestantes;

        // 🔥 Tempo de bloqueio local
        int tempoBloqueioSegundos = 60;
        DispatcherTimer timerDesbloqueio;


        public MainWindow()
        {
            InitializeComponent();
            connectionString = $"SERVER={servidor}; PORT=3306; DATABASE={bd}; UID={usr}; PASSWORD={senha};";

            tentativasRestantes = maxTentativas;

            // Timer para desbloqueio
            timerDesbloqueio = new DispatcherTimer();
            timerDesbloqueio.Interval = TimeSpan.FromSeconds(tempoBloqueioSegundos);
            timerDesbloqueio.Tick += TimerDesbloqueio_Tick;

            // Carregar credenciais
            if (Properties.Settings.Default.Lembrar)
            {
                textBoxEmail.Text = Properties.Settings.Default.Email;

                try
                {
                    textBoxSenha.Password = Decrypt(Properties.Settings.Default.Senha);
                    checkBoxRememberMe.IsChecked = true;
                }
                catch
                {
                    Properties.Settings.Default.Lembrar = false;
                    Properties.Settings.Default.Senha = "";
                    Properties.Settings.Default.Save();
                }
            }

            _ = CheckForSignedUpdateAsync();
        }

        private void TimerDesbloqueio_Tick(object sender, EventArgs e)
        {
            timerDesbloqueio.Stop();
            tentativasRestantes = maxTentativas;
            btnLogin.IsEnabled = true;

            CustomMessageBox.Show("Desbloqueado",
                "Você pode tentar fazer login novamente.");
        }


        private string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return "";
            var bytes = Encoding.UTF8.GetBytes(plainText);
            var protectedBytes = ProtectedData.Protect(bytes, null, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(protectedBytes);
        }

        private string Decrypt(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText))
                return "";
            var bytes = Convert.FromBase64String(encryptedText);
            var unprotectedBytes = ProtectedData.Unprotect(bytes, null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(unprotectedBytes);
        }


        private async Task CheckForSignedUpdateAsync()
        {
            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.UserAgent.ParseAdd("request");

                var url = $"https://api.github.com/repos/{RepoOwner}/{RepoName}/releases/latest";
                var response = await client.GetStringAsync(url);

                using var doc = JsonDocument.Parse(response);
                var latestVersion = doc.RootElement.GetProperty("tag_name").GetString();
                var assets = doc.RootElement.GetProperty("assets").EnumerateArray();

                string msixUrl = null;
                foreach (var asset in assets)
                {
                    var name = asset.GetProperty("name").GetString();
                    if (name.EndsWith(".msix"))
                    {
                        msixUrl = asset.GetProperty("browser_download_url").GetString();
                        break;
                    }
                }

                if (IsNewerVersion(latestVersion, CurrentVersion) && msixUrl != null)
                {
                    var result = MessageBox.Show(
                        $"Nova versão ({latestVersion}) disponível! Baixar agora?",
                        "Atualização disponível",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Information);

                    if (result == MessageBoxResult.Yes)
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = msixUrl,
                            UseShellExecute = true
                        });
                    }
                }
            }
            catch { }
        }

        private bool IsNewerVersion(string latest, string current)
        {
            if (string.IsNullOrEmpty(latest) || string.IsNullOrEmpty(current)) return false;

            Version vLatest = new Version(latest.TrimStart('v'));
            Version vCurrent = new Version(current.TrimStart('v'));
            return vLatest > vCurrent;
        }

        private void click(object sender, MouseButtonEventArgs e)
        {
            login_esq_senha login = new login_esq_senha(servidor, bd, usr, senha);
            login.Show();
            this.Close();
        }


        private void textBoxSenha_PasswordChanged(object sender, RoutedEventArgs e)
        {
            pwdWatermark.Visibility =
                string.IsNullOrEmpty(textBoxSenha.Password) ?
                Visibility.Visible : Visibility.Collapsed;
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string email = textBoxEmail.Text;
            string password = textBoxSenha.Password;

            var authService = new AuthService(servidor, bd, usr, senha);

            try
            {
                if (authService.Login(email, password))
                {
                    // 🔥 Reset tentativas
                    tentativasRestantes = maxTentativas;

                    CustomMessageBox.Show("Sucesso", "Login realizado com sucesso!");

                    if (checkBoxRememberMe.IsChecked == true)
                    {
                        Properties.Settings.Default.Email = email;
                        Properties.Settings.Default.Senha = Encrypt(password);
                        Properties.Settings.Default.Lembrar = true;
                    }
                    else
                    {
                        Properties.Settings.Default.Email = "";
                        Properties.Settings.Default.Senha = "";
                        Properties.Settings.Default.Lembrar = false;
                    }
                    Properties.Settings.Default.Save();

                    string permissao = authService.ObterPermissao(email, password);

                    Sessao.servidor = servidor;
                    Sessao.senha = senha;
                    Sessao.bd = bd;
                    Sessao.usr = usr;
                    Sessao.email = email;

                    if (permissao == "usr")
                    {
                        Sessao.Permissao = "usr";
                        new homePage_usr(email, servidor, bd, usr, senha).Show();
                        this.Close();
                    }
                    else if (permissao == "adm")
                    {
                        Sessao.Permissao = "adm";
                        new homePage_adm(email, servidor, bd, usr, senha).Show();
                        this.Close();
                    }
                    else if (permissao == "ges")
                    {
                        Sessao.Permissao = "ges";
                        new homePage_ges(email, servidor, bd, usr, senha).Show();
                        this.Close();
                    }
                    else
                    {
                        CustomMessageBox.Show("Atenção", "Permissão desconhecida.");
                    }
                }
                else
                {
                    tentativasRestantes--;

                    if (tentativasRestantes > 0)
                    {
                        CustomMessageBox.Show(
                            "Erro de Login",
                            $"Credenciais inválidas.\nTentativas restantes: {tentativasRestantes}");
                    }
                    else
                    {
                        CustomMessageBox.Show(
                            "Bloqueado",
                            $"Máximo de tentativas excedido.\nTente novamente em {tempoBloqueioSegundos} segundos.");

                        btnLogin.IsEnabled = false;
                        timerDesbloqueio.Start();
                    }

                    return;
                }
            }
            catch (MySqlException ex)
            {
                CustomMessageBox.Show("Erro de Conexão", ex.Message);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Erro Inesperado", ex.Message);
            }
        }
    }
}

