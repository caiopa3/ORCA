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
using System.Data;
using ORCA.Services;

namespace ORCA
{
    /// <summary>
    /// Lógica interna para gereFunc_adm.xaml
    /// </summary>
    public partial class gereFunc_adm : Window
    {
        public string servidor = "";
        public string bd = "";
        public string usr = "";
        public string senha = "";
        public string email = "";

        private OrcamentoService service;

        public gereFunc_adm(string e, string s, string b, string u, string se)
        {
            InitializeComponent();
            email = e;
            servidor = s;
            bd = b;
            usr = u;
            senha = se;

            // passa a config do banco no construtor
            service = new OrcamentoService(servidor, bd, usr, senha);

            CarregarUsuariosNaTela();

            cmb_camp_func.ItemsSource = new List<string> { "id", "email", "permissao" };
            cmb_camp_func.SelectedIndex = 0; // seleciona o primeiro por padrão

        }

        private void CarregarUsuariosNaTela()
        {
            try
            {
                dgUsuarios.ItemsSource = service.CarregarUsuarios().DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar usuários: " + ex.Message);
            }
        }

        private void btn_cad_func_Click(object sender, RoutedEventArgs e)
        {
            cadFunc_adm cadFunc_Adm = new cadFunc_adm(email, servidor, bd, usr, senha);
            cadFunc_Adm.Show();
            this.Close();
        }

        private void btn_fil_func_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string campo = cmb_camp_func.SelectedItem?.ToString();
                string valor = txtFiltro.Text.Trim(); // <- renomear o TextBox no XAML para txtFiltro

                if (string.IsNullOrWhiteSpace(campo) || string.IsNullOrWhiteSpace(valor))
                {
                    MessageBox.Show("Selecione o campo e digite um valor para filtrar.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                dgUsuarios.ItemsSource = service.FiltrarUsuarios(campo, valor).DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao filtrar usuários: " + ex.Message);
            }
        }

        private void txtFiltro_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFiltro.Text))
            {
                // se apagar tudo, recarrega os usuários normais
                CarregarUsuariosNaTela();
            }
        }

        private void BtnAlterar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is DataRowView row)
            {
                string emailFuncionario = row["email"].ToString();

                altFunc_adm alterar = new altFunc_adm(emailFuncionario, service);
                bool? resultado = alterar.ShowDialog();

                if (resultado == true)
                {
                    CarregarUsuariosNaTela();
                }
            }
        }

        private void BtnExcluir_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is DataRowView row)
            {
                string emailFuncionario = row["email"].ToString();

                var confirm = MessageBox.Show($"Deseja realmente excluir o funcionário {emailFuncionario}?", "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (confirm == MessageBoxResult.Yes)
                {
                    try
                    {
                        using var conn = new MySqlConnection($"SERVER={servidor};PORT=3306;DATABASE={bd};UID={usr};PASSWORD={senha};");
                        conn.Open();

                        string sql = "DELETE FROM usuario WHERE email=@Email";
                        using var cmd = new MySqlCommand(sql, conn);
                        cmd.Parameters.AddWithValue("@Email", emailFuncionario);
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Funcionário excluído com sucesso!");
                        CarregarUsuariosNaTela();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro ao excluir funcionário: " + ex.Message);
                    }
                }
            }
        }
    }
}
