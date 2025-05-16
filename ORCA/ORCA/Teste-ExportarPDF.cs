using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;

namespace ORCA
{
    public partial class Teste_ExportarPDF : Form
    {
        public Teste_ExportarPDF()
        {
            InitializeComponent();
            // Criar e configurar DataGridView
            dataGridView1 = new DataGridView { Dock = DockStyle.Top, Height = 200 };
            this.Controls.Add(dataGridView1);

            // Adicionar um botão para gerar PDF
            Button button1 = new Button { Text = "Gerar PDF", Dock = DockStyle.Top };
            button1.Click += button1_Click;
            this.Controls.Add(button1);

            // Preencher o DataGridView com dados fictícios
            PreencherDados();
        }

        private void PreencherDados()
        {
            DataTable table = new DataTable();
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("Nome", typeof(string));
            table.Columns.Add("Idade", typeof(int));

            table.Rows.Add(1, "João", 30);
            table.Rows.Add(2, "Maria", 25);
            table.Rows.Add(3, "Carlos", 35);

            dataGridView1.DataSource = table;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GerarPDF();
        }

        private void GerarPDF()
        {
            Document doc = new Document();
            Section section = doc.AddSection();
            Table migraTable = section.AddTable();
            migraTable.Borders.Width = 0.75;

            // Criar colunas
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                migraTable.AddColumn("4cm");
            }

            // Cabeçalho da tabela
            Row headerRow = migraTable.AddRow();
            headerRow.Format.Font.Bold = true;
            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                headerRow.Cells[i].AddParagraph(dataGridView1.Columns[i].HeaderText);
            }

            // Adicionar linhas com os dados do DataGridView
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (!row.IsNewRow)
                {
                    Row dataRow = migraTable.AddRow();
                    for (int i = 0; i < dataGridView1.Columns.Count; i++)
                    {
                        dataRow.Cells[i].AddParagraph(row.Cells[i].Value?.ToString() ?? "");
                    }
                }
            }

            // Renderizar PDF
            PdfDocumentRenderer renderer = new PdfDocumentRenderer(true);
            renderer.Document = doc;
            renderer.RenderDocument();

            // Salvar o PDF na área de trabalho
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string pdfPath = Path.Combine(desktopPath, "Tabela_Demo.pdf");
            renderer.PdfDocument.Save(pdfPath);
            MessageBox.Show("PDF salvo em: " + pdfPath, "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
