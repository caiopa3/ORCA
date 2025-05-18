using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient; 
using System.IO;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Pdf;
using System.Data.SqlClient;

namespace ORCA
{
    public partial class Login : Form
    {
        public string servidor = "srv1889.hstgr.io";
        public string bd = "u202947255_orca";
        public string usr = "u202947255_root";
        public string senha = "TCCorca123";
        public string connectionString;

        public Login()
        {
            InitializeComponent();
            connectionString = $"SERVER={servidor}; PORT=3306; DATABASE={bd}; UID={usr}; PASSWORD={senha};";
            MessageBox.Show(connectionString);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string email = textBoxUsername.Text;
            string password = textBoxPassword.Text;

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
    }
}
