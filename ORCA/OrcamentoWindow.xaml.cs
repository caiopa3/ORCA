using ORCA.Services;
using System.Data;
using System.Windows;

namespace ORCA
{
    public partial class OrcamentoWindow : Window
    {
        private readonly int _orcamentoId;
        private readonly OrcamentoService _orcamentoService;

        public OrcamentoWindow(int orcamentoId, OrcamentoService orcamentoService)
        {
            InitializeComponent();
            _orcamentoId = orcamentoId;
            _orcamentoService = orcamentoService;

            Carregar();
        }

        private void Carregar()
        {
            try
            {
                // 1) Busca o JSON do modelo associado a este orçamento
                string json = _orcamentoService.CarregarModeloJsonPorOrcamentoId(_orcamentoId);
                if (string.IsNullOrWhiteSpace(json))
                {
                    MessageBox.Show("Nenhum modelo encontrado para este orçamento.");
                    return;
                }

                // 2) Converte o JSON (Colunas/Linhas) para DataTable e mostra no grid
                DataTable tabela = _orcamentoService.ModeloJsonParaDataTable(json);
                dataGridOrcamento.ItemsSource = tabela.DefaultView;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Erro ao carregar orçamento: " + ex.Message);
            }
        }
    }
}
