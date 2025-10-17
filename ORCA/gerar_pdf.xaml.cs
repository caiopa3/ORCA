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
        public string tituloJanela;

        public gerar_pdf(DataTable data, string nome)
        {
            InitializeComponent();
            _data = data;
            tituloJanela = nome;
            this.Title = $"Exportar para PDF - {tituloJanela}";

            // Permite usar as fontes do Windows automaticamente
            PdfSharp.Fonts.GlobalFontSettings.UseWindowsFontsUnderWindows = true;
        }

        private void BtnExportPdf_Click(object sender, RoutedEventArgs e)
        {
            var contentWindow = new PdfContentWindow(tituloJanela);
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

                double margem = 40; // margem geral

                // ======== CABEÇALHO ========
                XImage header = XImage.FromFile(headerPath);

                double imgHeaderWidth = header.PixelWidth * 72.0 / header.HorizontalResolution;
                double imgHeaderHeight = header.PixelHeight * 72.0 / header.HorizontalResolution;

                double maxHeaderHeight = XUnit.FromCentimeter(3.5).Point;
                double visibleHeaderHeight = Math.Min(imgHeaderHeight, maxHeaderHeight);
                double visibleHeaderWidth = Math.Min(imgHeaderWidth, page.Width);

                // Aviso se redimensionado
                if (visibleHeaderHeight != imgHeaderHeight || visibleHeaderWidth != imgHeaderWidth)
                {
                    MessageBox.Show(
                        "A imagem do cabeçalho foi redimensionada automaticamente para caber no layout.",
                        "Aviso",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
                }

                // Centraliza horizontalmente
                double headerX = (page.Width - visibleHeaderWidth) / 2;
                // Se imagem maior que página, define corte central
                double sourceHeaderX = imgHeaderWidth > page.Width ? (imgHeaderWidth - page.Width) / 2 : 0;

                gfx.DrawImage(header,
                    new XRect(headerX, 0, visibleHeaderWidth, visibleHeaderHeight),
                    new XRect(sourceHeaderX, 0, visibleHeaderWidth, visibleHeaderHeight),
                    XGraphicsUnit.Point);

                // ======== RODAPÉ ========
                XImage footer = XImage.FromFile(footerPath);

                double imgFooterWidth = footer.PixelWidth * 72.0 / footer.HorizontalResolution;
                double imgFooterHeight = footer.PixelHeight * 72.0 / footer.HorizontalResolution;

                double maxFooterHeight = XUnit.FromCentimeter(3.5).Point;
                double visibleFooterHeight = Math.Min(imgFooterHeight, maxFooterHeight);
                double visibleFooterWidth = Math.Min(imgFooterWidth, page.Width);

                // Aviso se redimensionado
                if (visibleFooterHeight != imgFooterHeight || visibleFooterWidth != imgFooterWidth)
                {
                    MessageBox.Show(
                        "A imagem do rodapé foi redimensionada automaticamente para caber no layout.",
                        "Aviso",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
                }

                double footerX = (page.Width - visibleFooterWidth) / 2;
                double footerY = page.Height - visibleFooterHeight;
                double sourceFooterX = imgFooterWidth > page.Width ? (imgFooterWidth - page.Width) / 2 : 0;

                gfx.DrawImage(footer,
                    new XRect(footerX, footerY, visibleFooterWidth, visibleFooterHeight),
                    new XRect(sourceFooterX, 0, visibleFooterWidth, visibleFooterHeight),
                    XGraphicsUnit.Point);


                // ======== TEXTO ========
                var font = new XFont("Arial", 14, XFontStyleEx.Regular);
                XTextFormatter tf = new XTextFormatter(gfx);

                double textoY = visibleHeaderHeight + margem;
                double textoAlturaDisponivel = page.Height - visibleHeaderHeight - visibleFooterHeight - 2 * margem;

                XRect textRect = new XRect(margem, textoY, page.Width - 2 * margem, textoAlturaDisponivel);
                tf.DrawString(pdfText, font, XBrushes.Black, textRect, XStringFormats.TopLeft);

                // ======== TABELA ========
                int linhasTexto = pdfText.Split('\n').Length;
                double alturaTexto = linhasTexto * font.GetHeight();
                double startY = textoY + alturaTexto + 20;

                if (_data != null && _data.Columns.Count > 0)
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
                                new XRect(margem + j * colWidth, startY + (i + 1) * rowHeight, colWidth, rowHeight),
                                XStringFormats.Center);
                        }
                    }
                }

                // ======== SALVAR PDF ========
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

        private void btnVoltar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
