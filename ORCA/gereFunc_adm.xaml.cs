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
    }
}
