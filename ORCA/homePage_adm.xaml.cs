using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

namespace ORCA
{
    /// <summary>
    /// Lógica interna para homePage_adm.xaml
    /// </summary>
    public partial class homePage_adm : Window
    {
        public string servidor = "";
        public string bd = "";
        public string usr = "";
        public string senha = "";
        public string email = "";

        public homePage_adm(string e, string s, string b, string u, string se)
        {
            InitializeComponent();
            email = e;
            servidor = s;
            bd = b;
            usr = u;
            senha = se;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            criar_orca criarOrcamento = new criar_orca(email, servidor, bd, usr, senha);
            criarOrcamento.Show();
            this.Close();
        }
    }
}
