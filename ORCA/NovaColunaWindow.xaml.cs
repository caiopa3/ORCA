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
    /// Lógica interna para NovaColunaWindow.xaml
    /// </summary>
    public partial class NovaColunaWindow : Window
    {
        public string NomeColuna { get; private set; }
        public string TipoDado { get; private set; }
        public string Operacao { get; private set; }
        public string Relacionamento { get; private set; }

        public NovaColunaWindow()
        {
            InitializeComponent();
        }

        private void Confirmar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNomeColuna.Text))
            {
                MessageBox.Show("Nome da coluna é obrigatório.");
                return;
            }

            NomeColuna = txtNomeColuna.Text.Trim();
            TipoDado = (cmbTipoDado.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Texto";
            Operacao = txtOperacao.Text.Trim();
            Relacionamento = txtRelacionamento.Text.Trim();
            this.DialogResult = true;
            this.Close();
        }

        private void cmbTipoDado_DropDownClosed(object sender, EventArgs e)
        {
            if (cmbTipoDado.Text == "Número")
            {
                txtBlockOperacao.Visibility = Visibility.Visible;
                txtBlockRelacionamento.Visibility = Visibility.Visible;
                txtOperacao.Visibility = Visibility.Visible;
                txtRelacionamento.Visibility = Visibility.Visible;
            }
            else
            {
                txtOperacao.Visibility = Visibility.Collapsed;
                txtRelacionamento.Visibility = Visibility.Collapsed;
            }
        }
    }
}
