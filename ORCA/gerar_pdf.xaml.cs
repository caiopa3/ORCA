using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Diagnostics;
using PdfSharp.Drawing.Layout;
using System.Data;

namespace ORCA
{
    /// <summary>
    /// Lógica interna para gerar_pdf.xaml
    /// </summary>
    public partial class gerar_pdf : Window
    {
        private string headerPath;
        private string footerPath;
        private readonly DataTable _data;
        public gerar_pdf(DataTable data)
        {
            InitializeComponent();
            _data = data;

            // Permite usar as fontes do Windows automaticamente
            PdfSharp.Fonts.GlobalFontSettings.UseWindowsFontsUnderWindows = true;
        }

        private void BtnExportPdf_Click(object sender, RoutedEventArgs e)
        {
            var contentWindow = new PdfContentWindow();
            if (contentWindow.ShowDialog() != true)
                return;

            string pdfText = contentWindow.PdfContent;

            try
            {
                if (string.IsNullOrEmpty(headerPath) || string.IsNullOrEmpty(footerPath))
                {
                    MessageBox.Show("Por favor, carregue as imagens de cabeçalho e rodapé.", "Aviso",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var saveDlg = new SaveFileDialog
                {
                    Filter = "PDF (*.pdf)|*.pdf",
                    FileName = "PDFPersonalizado.pdf"
                };

                if (saveDlg.ShowDialog() != true)
                    return;

                string filename = saveDlg.FileName;

                PdfDocument document = new PdfDocument();
                document.Info.Title = "PDF Personalizado";

                PdfPage page = document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);

                // Cabeçalho
                XImage header = XImage.FromFile(headerPath);
                double headerMaxWidth = page.Width;
                double headerHeight = header.PixelHeight * headerMaxWidth / header.PixelWidth;
                gfx.DrawImage(header, 0, 0, headerMaxWidth, headerHeight);

                // Rodapé
                XImage footer = XImage.FromFile(footerPath);
                double footerMaxWidth = page.Width;
                double footerHeight = footer.PixelHeight * footerMaxWidth / footer.PixelWidth;
                gfx.DrawImage(footer, 0, page.Height - footerHeight, footerMaxWidth, footerHeight);

                // ---- Desenha o texto primeiro ----
                var font = new XFont("Arial", 14, XFontStyleEx.Regular);
                XTextFormatter tf = new XTextFormatter(gfx);

                double margem = 40;
                double textoY = headerHeight + margem;
                double textoAlturaDisponivel = page.Height - headerHeight - footerHeight - 2 * margem;

                XRect textRect = new XRect(margem, textoY, page.Width - 2 * margem, textoAlturaDisponivel);
                tf.DrawString(pdfText, font, XBrushes.Black, textRect, XStringFormats.TopLeft);

                // ---- Calcula altura do texto ----
                int linhasTexto = pdfText.Split('\n').Length;
                double alturaTexto = linhasTexto * font.GetHeight();

                // Posição inicial da tabela (logo após o texto)
                double startY = textoY + alturaTexto + 20;

                if (_data != null)
                {
                    XFont tableFont = new XFont("Arial", 12, XFontStyleEx.Regular);
                    int rowHeight = 20;
                    int colWidth = (int)((page.Width - 2 * margem) / _data.Columns.Count);

                    // Cabeçalho da tabela
                    for (int j = 0; j < _data.Columns.Count; j++)
                    {
                        gfx.DrawRectangle(XPens.Black, XBrushes.LightGray,
                            margem + j * colWidth, startY, colWidth, rowHeight);

                        gfx.DrawString(_data.Columns[j].ColumnName, tableFont, XBrushes.Black,
                            new XRect(margem + j * colWidth, startY, colWidth, rowHeight), XStringFormats.Center);
                    }

                    // Linhas da tabela
                    for (int i = 0; i < _data.Rows.Count; i++)
                    {
                        for (int j = 0; j < _data.Columns.Count; j++)
                        {
                            gfx.DrawRectangle(XPens.Black, XBrushes.White,
                                margem + j * colWidth, startY + (i + 1) * rowHeight, colWidth, rowHeight);

                            gfx.DrawString(_data.Rows[i][j]?.ToString(), tableFont, XBrushes.Black,
                                new XRect(margem + j * colWidth, startY + (i + 1) * rowHeight, colWidth, rowHeight), XStringFormats.Center);
                        }
                    }
                }

                document.Save(filename);
                Process.Start("explorer.exe", System.IO.Path.GetFullPath(filename));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao gerar PDF: " + ex.Message);
            }
        }

        private void BtnLoadHeader_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Imagens (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg"
            };

            if (dlg.ShowDialog() == true)
            {
                headerPath = dlg.FileName;
                ImgHeader.Source = new BitmapImage(new Uri(headerPath));
            }
        }

        private void BtnLoadFooter_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Imagens (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg"
            };

            if (dlg.ShowDialog() == true)
            {
                footerPath = dlg.FileName;
                ImgFooter.Source = new BitmapImage(new Uri(footerPath));
            }
        }
    }
}
