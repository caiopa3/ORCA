using PdfSharp.Pdf.Advanced;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
using System.Data;

namespace ORCA
{
    /// <summary>
    /// Lógica interna para PdfContentWindow.xaml
    /// </summary>
    public partial class PdfContentWindow : Window
    {
        public string PdfContent { get; private set; }
        public string tituloJanela;

        public PdfContentWindow(string nome)
        {
            InitializeComponent();
            tituloJanela = nome;
            this.Title = $"Conteúdo do PDF - {tituloJanela}";
        }

        private void BtnGeneratePdf_Click(object sender, RoutedEventArgs e)
        {
            PdfContent = TxtPdfContent.Text.Trim();
            if (string.IsNullOrEmpty(PdfContent))
            {
                MessageBox.Show("Digite algum conteúdo para o PDF.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            DialogResult = true; // Fecha a janela e retorna true
        }

        private void btnVoltar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
