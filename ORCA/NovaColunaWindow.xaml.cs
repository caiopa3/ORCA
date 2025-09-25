using System;
using System.Windows;
using System.Windows.Controls;

namespace ORCA
{
    /// <summary>
    /// Lógica interna para NovaColunaWindow.xaml
    /// </summary>
    public partial class NovaColunaWindow : Window
    {
        public string NomeColuna { get; private set; }
        public string TipoDado { get; private set; }
        public string ValorFixo { get; private set; }

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
            ValorFixo = chkValorFixo.IsChecked == true ? txtValorFixo.Text.Trim() : null;
            this.DialogResult = true;
            this.Close();
        }

        private void chkValorFixo_Checked(object sender, RoutedEventArgs e)
        {
            borderValorFixo.Visibility = Visibility.Visible;
        }

        private void chkValorFixo_Unchecked(object sender, RoutedEventArgs e)
        {
            borderValorFixo.Visibility = Visibility.Collapsed;
        }
    }
}
