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

            using (MySqlConnection conexao = new MySqlConnection(connectionString))
            {
                try
                {
                    conexao.Open();

                    string query = "SELECT COUNT(*) FROM usuario WHERE email = @email AND senha = @password";
                    using (MySqlCommand comando = new MySqlCommand(query, conexao))
                    {
                        comando.Parameters.AddWithValue("@email", email);
                        comando.Parameters.AddWithValue("@password", password);

                        object resultado = comando.ExecuteScalar();
                        int count = Convert.ToInt32(resultado);

                        if (count > 0)
                        {
                            MessageBox.Show("Login realizado com sucesso!");

                            string query_permissao = "SELECT permissao FROM usuario WHERE email = @email AND senha = @password";
                            MySqlCommand comando_permissao = new MySqlCommand(query_permissao, conexao);
                            comando_permissao.Parameters.AddWithValue("@email", email);
                            comando_permissao.Parameters.AddWithValue("@password", password);
                            object resultado_permissao = comando_permissao.ExecuteScalar();

                            string permissao = Convert.ToString(resultado_permissao);

                            if (permissao == "usr")
                            {
                                homePage_usr homePage_Usr = new homePage_usr(email);
                                homePage_Usr.Show();
                                this.Close();
                            } // Falta o do adm
                        }
                        else
                        {
                            MessageBox.Show("Usuário ou senha incorretos.");
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"Erro ao conectar no banco de dados:\n{ex.Message}");
                }
            }
        }

        private void click(object sender, MouseButtonEventArgs e)
        {
            login_esq_senha login_esq_senha = new login_esq_senha();
            login_esq_senha.Show();
            this.Close();
        }
    }
}