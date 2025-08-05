using System;
using System.Collections.Generic;
using System.Data;
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
    /// Lógica interna para criar_orca.xaml
    /// </summary>
    public partial class criar_orca : Window
    {
        private DataTable tabela;

        public criar_orca()
        {
            InitializeComponent();

            // Inicializa a tabela
            tabela = new DataTable();

            // Exemplo de coluna inicial
            tabela.Columns.Add("SampleInt");
            tabela.Columns.Add("SampleStringA");
            tabela.Columns.Add("SampleStringB");

            // Preenche algumas linhas de exemplo
            for (int i = 1; i <= 5; i++)
            {
                var row = tabela.NewRow();
                row["SampleInt"] = i;
                row["SampleStringA"] = $"Amostra da Cadeia de caracteres A - {i}";
                row["SampleStringB"] = $"Amostra da Cadeia de caracteres B - {i}";
                tabela.Rows.Add(row);
            }

            dataGrid.ItemsSource = tabela.DefaultView;
        }


        private void AdicionarLinha_Click(object sender, RoutedEventArgs e)
        {
            var novaLinha = tabela.NewRow();

            // Se tiver texto na TextBox da linha, preencher a primeira coluna
            string textoLinha = txtNomeLinha.Text.Trim();
            if (!string.IsNullOrEmpty(textoLinha))
            {
                if (tabela.Columns.Count > 0)
                    novaLinha[0] = textoLinha;
            }

            tabela.Rows.Add(novaLinha);
        }

        private void AdicionarColuna_Click(object sender, RoutedEventArgs e)
        {
            string nomeColuna = txtNomeColuna.Text.Trim();

            if (string.IsNullOrEmpty(nomeColuna))
            {
                MessageBox.Show("Digite o nome da coluna.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (tabela.Columns.Contains(nomeColuna))
            {
                MessageBox.Show("Já existe uma coluna com esse nome.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            tabela.Columns.Add(nomeColuna);
        }

        private void txtNomeLinha_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
