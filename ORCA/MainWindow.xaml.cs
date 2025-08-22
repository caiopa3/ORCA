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

namespace ORCA
{

    public partial class MainWindow : Window
    {
        public string servidor = "srv1889.hstgr.io";
        public string bd = "u202947255_orca";
        public string usr = "u202947255_root";
        public string senha = "TCCorca123";
        public string connectionString;
        public MainWindow()
        {
            InitializeComponent();
            connectionString = $"SERVER={servidor}; PORT=3306; DATABASE={bd}; UID={usr}; PASSWORD={senha};";
            MessageBox.Show(connectionString);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string email = textBoxEmail.Text;
            string password = textBoxSenha.Text;

            var authService = new ORCA.Services.AuthService(servidor, bd, usr, senha);

            try
            {
                if (authService.ValidarLogin(email, password))
                {
                    MessageBox.Show("Login realizado com sucesso!");

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


        private void click(object sender, MouseButtonEventArgs e)
        {
            login_esq_senha login_esq_senha = new login_esq_senha(servidor, bd, usr, senha);
            login_esq_senha.Show();
            this.Close();
        }
    }

}
