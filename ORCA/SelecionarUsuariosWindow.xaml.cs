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
using System.Collections.Generic;

namespace ORCA
{
    /// <summary>
    /// Lógica interna para SelecionarUsuariosWindow.xaml
    /// </summary>
    public partial class SelecionarUsuariosWindow : Window
    {
        public List<int> UsuariosSelecionados { get; private set; } = new List<int>();

        public SelecionarUsuariosWindow()
        {
            InitializeComponent();
            CarregarUsuarios();
        }

        private void Confirmar_Click(object sender, RoutedEventArgs e)
        {
            if (listaUsuarios.SelectedItems.Count == 0)
            {

                MessageBox.Show("Selecione um usuário!");

            }

            else {

                foreach (var item in listaUsuarios.SelectedItems)
                {
                    var usuario = item as UsuarioInfo;
                    if (usuario != null)
                        UsuariosSelecionados.Add(usuario.Id);
                }

                this.DialogResult = true;
                this.Close();

            }

        }

        private void CarregarUsuarios()
        {
            try
            {
                using (var conn = new MySqlConnection("SERVER=localhost;DATABASE=banco;UID=root;PWD=;"))
                {
                    conn.Open();
                    var cmd = new MySqlCommand("SELECT id, email FROM view_decripto", conn);
                    var reader = cmd.ExecuteReader();

                    var lista = new List<UsuarioInfo>();

                    while (reader.Read())
                    {
                        lista.Add(new UsuarioInfo
                        {
                            Id = reader.GetInt32("id"),
                            Email = reader.GetString("email")
                        });
                    }

                    listaUsuarios.ItemsSource = lista;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar usuários: " + ex.Message);
            }
        }
        private class UsuarioInfo
        {
            public int Id { get; set; }
            public string Email { get; set; }
        }
    }
}
