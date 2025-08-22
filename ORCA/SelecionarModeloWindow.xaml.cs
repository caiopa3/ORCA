using System.Collections.Generic;
using System.Windows;

namespace ORCA
{
    public partial class SelecionarModeloWindow : Window
    {
        public class ModeloItem
        {
            public int Id { get; set; }
            public string Nome { get; set; }
        }

        public ModeloItem ModeloSelecionado { get; private set; }

        public SelecionarModeloWindow(List<ModeloItem> modelos)
        {
            InitializeComponent();
            listModelos.ItemsSource = modelos;
        }

        private void BtnConfirmar_Click(object sender, RoutedEventArgs e)
        {
            ModeloSelecionado = listModelos.SelectedItem as ModeloItem;
            if (ModeloSelecionado == null)
            {
                MessageBox.Show("Selecione um modelo!");
                return;
            }
            DialogResult = true;
            Close();
        }
    }
}
