using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
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
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace ORCA
{
    /// <summary>
    /// Lógica interna para criar_orca.xaml
    /// </summary>
    public partial class criar_orca : Window
    {
        private ObservableCollection<ExpandoObject> dados = new ObservableCollection<ExpandoObject>();
        public string servidor = "";
        public string bd = "";
        public string usr = "";
        public string senha = "";
        public string email = "";

        private int usuarioCriadorId;

        public criar_orca(string e, string s, string b, string u, string se)
        {
            InitializeComponent();

            email = e;
            servidor = s;
            bd = b;
            usr = u;
            senha = se;

            string connectionString = $"SERVER={servidor}; PORT=3306; DATABASE={bd}; UID={usr}; PASSWORD={senha};";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT id FROM usuario WHERE email = @Email";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);

                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        usuarioCriadorId = Convert.ToInt32(result);
                    }
                }
            }

            meuDataGrid.ItemsSource = dados;

            // Adiciona linha inicial
            dynamic linha = new ExpandoObject();
            dados.Add(linha);
        }

        private void AdicionarColuna_Click(object sender, RoutedEventArgs e)
        {
            var janela = new NovaColunaWindow();

            if (janela.ShowDialog() == true)
            {
                string nomeColuna = janela.NomeColuna;

                var novaColuna = new DataGridTextColumn
                {
                    Header = $"{nomeColuna} ({janela.TipoDado})",
                    Binding = new System.Windows.Data.Binding(nomeColuna)
                };

                meuDataGrid.Columns.Add(novaColuna);

                foreach (IDictionary<string, object> linha in dados)
                {
                    if (!linha.ContainsKey(nomeColuna))
                        linha[nomeColuna] = "";
                }

                meuDataGrid.Items.Refresh();
            }
        }

        private void SalvarModelo_Click(object sender, RoutedEventArgs e)
        {
            SalvarTabelaNoBanco();
        }

        private void SalvarTabelaNoBanco()
        {
            try
            {
                string nomeModelo = Microsoft.VisualBasic.Interaction.InputBox("Digite o nome do modelo:", "Salvar Modelo", "Novo Modelo");

                if (string.IsNullOrWhiteSpace(nomeModelo))
                {
                    MessageBox.Show("Nome do modelo é obrigatório.");
                    return;
                }

                int modeloId;

                using (var conn = new MySqlConnection("SERVER=localhost;DATABASE=banco;UID=root;PWD=;"))
                {
                    conn.Open();

                    // 1️⃣ Criar modelo_orcamento
                    var cmdModelo = new MySqlCommand("INSERT INTO modelo_orcamento (nome, usr_criador_id) VALUES (@nome, @usr)", conn);
                    cmdModelo.Parameters.AddWithValue("@nome", nomeModelo);
                    cmdModelo.Parameters.AddWithValue("@usr", usuarioCriadorId);
                    cmdModelo.ExecuteNonQuery();

                    modeloId = (int)cmdModelo.LastInsertedId;

                    // 2️⃣ Salvar dados como JSON
                    var colunas = meuDataGrid.Columns.Select(c => c.Header.ToString()).ToList();
                    var linhas = dados.Select(linha =>
                        ((IDictionary<string, object>)linha)
                        .ToDictionary(k => k.Key, v => v.Value)
                    ).ToList();

                    var tabela = new
                    {
                        Colunas = colunas,
                        Linhas = linhas
                    };

                    string json = JsonConvert.SerializeObject(tabela);

                    var cmdDados = new MySqlCommand("INSERT INTO modelo_orcamento_dados (modelo_id, dados_json) VALUES (@mid, @json)", conn);
                    cmdDados.Parameters.AddWithValue("@mid", modeloId);
                    cmdDados.Parameters.AddWithValue("@json", json);
                    cmdDados.ExecuteNonQuery();
                }

                // 3️⃣ Seleção de usuários que terão acesso
                var selecaoUsuarios = new SelecionarUsuariosWindow();
                if (selecaoUsuarios.ShowDialog() == true)
                {
                    using (var conn = new MySqlConnection("SERVER=localhost;DATABASE=banco;UID=root;PWD=;"))
                    {
                        conn.Open();

                        foreach (var usuarioId in selecaoUsuarios.UsuariosSelecionados)
                        {
                            var cmdPermissao = new MySqlCommand("INSERT INTO modelo_orcamento_usuarios (modelo_id, usuario_id) VALUES (@mid, @uid)", conn);
                            cmdPermissao.Parameters.AddWithValue("@mid", modeloId);
                            cmdPermissao.Parameters.AddWithValue("@uid", usuarioId);
                            cmdPermissao.ExecuteNonQuery();
                        }
                    }
                }

                MessageBox.Show("Modelo salvo com sucesso!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar modelo: " + ex.Message);
            }
        }
    }
}
