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
using MySql.Data.MySqlClient;

namespace ORCA
{
    /// <summary>
    /// Lógica interna para homePage_usr.xaml
    /// </summary>
    public partial class homePage_usr : Window
    {
        public string servidor = "srv1889.hstgr.io";
        public string bd = "u202947255_orca";
        public string usr = "u202947255_root";
        public string senha = "TCCorca123";
        public string connectionString;
        public string email = "";

        private int contadorOrcamentos = 1;
        public homePage_usr(string e)
        {
            InitializeComponent();
            email = e;
            connectionString = $"SERVER={servidor}; PORT=3306; DATABASE={bd}; UID={usr}; PASSWORD={senha};";
            CarregarOrcamentosExistentes();
        }

        private void BtnCriarOrcamento_Click(object sender, RoutedEventArgs e)
        {
            InputBox("Criar Orçamento", "Digite o nome do orçamento:", "Novo Orçamento", nome =>
            {
                int id = InserirOrcamentoNoBanco(nome);
                if (id > 0)
                {
                    CriarOrcamentoVisual(id, nome);
                }
            });
        }


        private void CriarOrcamentoVisual(int id, string nome)
        {
            Border border = new Border
            {
                Width = 180,
                Height = 80,
                Margin = new Thickness(10),
                Background = Brushes.LightGray,
                CornerRadius = new CornerRadius(5),
                Padding = new Thickness(10)
            };

            StackPanel panel = new StackPanel();

            TextBlock nomeText = new TextBlock
            {
                Text = nome,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Cursor = Cursors.Hand,
                Tag = id
            };

            // nomeText.MouseDoubleClick += NomeText_MouseDoubleClick;

            Button btnEntrar = new Button
            {
                Content = "Entrar",
                Margin = new Thickness(0, 5, 0, 0),
                Tag = nome
            };

            btnEntrar.Click += (s, e) =>
            {
                MessageBox.Show($"Entrando em: {nome}");
            };

            panel.Children.Add(nomeText);
            panel.Children.Add(btnEntrar);
            border.Child = panel;

            wrapPanelOrcamentos.Children.Add(border);
        }

        private void NomeText_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock tb)
            {
                string nomeAtual = tb.Text;
                int id = (int)tb.Tag;

                InputBox("Renomear Orçamento", "Novo nome:", nomeAtual, novoNome =>
                {
                    if (!string.IsNullOrWhiteSpace(novoNome))
                    {
                        tb.Text = novoNome;
                        AtualizarNomeOrcamentoNoBanco(id, novoNome);
                    }
                });
            }
        }

        private int InserirOrcamentoNoBanco(string nome)
        {
            try
            {
                using MySqlConnection conn = new MySqlConnection(connectionString);
                conn.Open();
                string query = "INSERT INTO orcamentos (nome, usr_email) VALUES (@Nome, @Email); SELECT LAST_INSERT_ID();";
                using MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Nome", nome);
                cmd.Parameters.AddWithValue("@Email", email); // ← relaciona orçamento com o e-mail do usuário
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao inserir no banco: " + ex.Message);
                return -1;
            }
        }

        private void AtualizarNomeOrcamentoNoBanco(int id, string novoNome)
        {
            try
            {
                using MySqlConnection conn = new MySqlConnection(connectionString);
                conn.Open();
                string query = "UPDATE orcamentos SET nome = @Nome WHERE id = @Id";
                using MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Nome", novoNome);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao atualizar nome: " + ex.Message);
            }
        }

        private void CarregarOrcamentosExistentes()
        {
            try
            {
                using MySqlConnection conn = new MySqlConnection(connectionString);
                conn.Open();
                string query = "SELECT id, nome FROM orcamentos WHERE usr_email = @Email";
                using MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", email);
                using MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string nome = reader.GetString(1);
                    CriarOrcamentoVisual(id, nome);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar orçamentos: " + ex.Message);
            }
        }

        private void InputBox(string title, string prompt, string defaultValue, Action<string> onConfirm)
        {

            Window inputWindow = new Window
            {
                Title = title,
                Width = 300,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize
            };

            StackPanel panel = new StackPanel { Margin = new Thickness(10) };
            panel.Children.Add(new TextBlock { Text = prompt });

            TextBox input = new TextBox { Text = defaultValue };
            panel.Children.Add(input);

            Button okButton = new Button { Content = "OK", Margin = new Thickness(0, 10, 0, 0), Width = 60 };
            okButton.Click += (s, e) =>
            {
                onConfirm(input.Text.Trim());
                inputWindow.Close();
            };

            panel.Children.Add(okButton);
            inputWindow.Content = panel;
            inputWindow.ShowDialog();
        }
    }
}
