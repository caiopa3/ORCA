using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Security.Cryptography;


namespace ORCA
{
    public partial class MainWindow : Window
    {
        public string servidor = "localhost";
        public string bd = "banco";
        public string usr = "root";
        public string senha = "";
        public string connectionString;

        // Informações do GitHub
        private const string RepoOwner = "caiopa3";
        private const string RepoName = "ORCA";
        private const string CurrentVersion = "1.0.2"; // Atualize sempre que lançar uma nova versão

        public MainWindow()
        {
            InitializeComponent();
            connectionString = $"SERVER={servidor}; PORT=3306; DATABASE={bd}; UID={usr}; PASSWORD={senha};";
            MessageBox.Show(connectionString);

            // Carregar credenciais salvas
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
                    // Se falhar a descriptografia (mudança de usuário do Windows, por exemplo)
                    Properties.Settings.Default.Lembrar = false;
                    Properties.Settings.Default.Senha = "";
                    Properties.Settings.Default.Save();
                }
            }

            // Chama verificação de atualização assinada
            _ = CheckForSignedUpdateAsync();
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

                // Procura arquivo MSIX assinado no release
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
                        $"Uma nova versão assinada ({latestVersion}) está disponível! Deseja baixar agora?",
                        "Atualização disponível",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Information
                    );

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
            catch
            {
                // Falha na verificação não bloqueia login
            }
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
            login_esq_senha login_esq_senha = new login_esq_senha(servidor, bd, usr, senha);
            login_esq_senha.Show();
            this.Close();
        }
        private void textBoxSenha_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxSenha.Password))
                pwdWatermark.Visibility = Visibility.Visible;
            else
                pwdWatermark.Visibility = Visibility.Collapsed;
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string email = textBoxEmail.Text;
            string password = textBoxSenha.Password;

            var authService = new ORCA.Services.AuthService(servidor, bd, usr, senha);

            try
            {
                if (authService.ValidarLogin(email, password))
                {
                    MessageBox.Show("Login realizado com sucesso!");

                    // Salvar se "Lembrar-me" estiver marcado
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

                    if (permissao == "usr")
                    {
                        homePage_usr homePage_Usr = new homePage_usr(email, servidor, bd, usr, senha);
                        homePage_Usr.Show();
                        this.Close();
                    }
                    else if (permissao == "adm")
                    {
                        homePage_adm homePage_Adm = new homePage_adm(email, servidor, bd, usr, senha);
                        homePage_Adm.Show();
                        this.Close();
                    }
                    else if (permissao == "ges")
                    {
                        homePage_ges homePage_Ges = new homePage_ges(email, servidor, bd, usr, senha);
                        homePage_Ges.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Permissão desconhecida.");
                    }
                }
                else
                {
                    MessageBox.Show("Usuário ou senha incorretos.");
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Erro ao conectar no banco de dados:\n{ex.Message}");
            }
        }
    }
}
