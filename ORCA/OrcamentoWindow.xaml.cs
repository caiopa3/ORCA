using ORCA.Services;
using System.Data;
using System.Windows;

namespace ORCA
{
    public partial class OrcamentoWindow : Window
    {
        private readonly int _orcamentoId;
        private readonly OrcamentoService _orcamentoService;
        private readonly string _email;

        public OrcamentoWindow(int orcamentoId, OrcamentoService orcamentoService, string email)
        {
            InitializeComponent();
            _orcamentoId = orcamentoId;
            _orcamentoService = orcamentoService;
            _email = email;
            Carregar();
        }

        private void Carregar()
        {
            try
            {
                // 1) Busca o JSON do modelo associado a este orçamento
                string json = _orcamentoService.CarregarModeloJsonPorOrcamentoId(_orcamentoId, _email);
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

        private void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGridOrcamento.ItemsSource is DataView dv)
                {
                    DataTable tabela = dv.ToTable();
                    int usuarioId = _orcamentoService.ObterUsuarioIdPorEmail(_email);

                    _orcamentoService.SalvarDadosOrcamento(_orcamentoId, tabela, usuarioId);
                    MessageBox.Show("Orçamento salvo com sucesso!");
                }
                else
                {
                    MessageBox.Show("Não há dados para salvar.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar orçamento: " + ex.Message);
            }
        }

    }
}
