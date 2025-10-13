using ORCA.Services;
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

        private readonly OrcamentoService _orcamentoService;

        public homePage_adm(string e, string s, string b, string u, string se)
        {
            InitializeComponent();
            email = e;
            servidor = s;
            bd = b;
            usr = u;
            senha = se;

            _orcamentoService = new OrcamentoService(servidor, bd, usr, senha);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            criar_orca criarOrcamento = new criar_orca(email, servidor, bd, usr, senha);
            criarOrcamento.Show();
            this.Close();
        }

        private void btn_ger_func_Click(object sender, RoutedEventArgs e)
        {
            gereFunc_adm gereFunc_Adm = new gereFunc_adm(email, servidor, bd, usr, senha);
            gereFunc_Adm.Show();

        }

        private void btnVoltar_Click(object sender, RoutedEventArgs e)
        {
            // Fecha todas as janelas abertas, menos esta (para evitar erro de coleção modificada)
            foreach (Window janela in Application.Current.Windows)
            {
                if (janela != this)
                    janela.Close();
            }

            // Abre a tela inicial
            MainWindow telaInicial = new MainWindow();
            telaInicial.Show();

            // Fecha a janela atual por último
            this.Close();
        }

        private void btnPerfil_Click(object sender, RoutedEventArgs e)
        {
            var perfilWin = new PerfilWindow(email, _orcamentoService);
            bool? resultado = perfilWin.ShowDialog();

            if (resultado == true)
            {
                MessageBox.Show("Perfil atualizado com sucesso! Reinicie o sistema para aplicar as mudanças de login (se alterou e-mail).");
                email = Sessao.email; // Atualiza variável local também, se necessário

            }
        }
    }
}
