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

namespace ORCA
{
    /// <summary>
    /// Lógica interna para homePage_ges.xaml
    /// </summary>
    public partial class homePage_ges : Window
    {
        public string servidor = "";
        public string bd = "";
        public string usr = "";
        public string senha = "";

        public homePage_ges(string e, string s, string b, string u, string se)
        {
            InitializeComponent();

            servidor = s;
            bd = b;
            usr = u;
            senha = se;
        }
    }
}
