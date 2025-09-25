using ORCA.Services;
using System.Data;
using System.Windows;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ORCA
{
    public partial class OrcamentoWindow : Window
    {
        private Dictionary<(int rowIndex, string columnName), string> _formulas = new();

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

            dataGridOrcamento.CellEditEnding += DataGridOrcamento_CellEditEnding;
        }

        private void DataGridOrcamento_CellEditEnding(object sender, System.Windows.Controls.DataGridCellEditEndingEventArgs e)
        {
            var dataRowView = e.Row.Item as DataRowView;
            if (dataRowView == null) return;

            var columnName = e.Column.Header.ToString();
            var editor = e.EditingElement as System.Windows.Controls.TextBox;
            if (editor == null) return;

            string cellValue = editor.Text;
            int rowIndex = dataGridOrcamento.Items.IndexOf(dataRowView);

            if (cellValue.StartsWith("="))
            {
                _formulas[(rowIndex, columnName)] = cellValue;
                RecalcularFormulasLinha(dataRowView, rowIndex);
            }
            else
            {
                _formulas.Remove((rowIndex, columnName));
                RecalcularFormulasLinha(dataRowView, rowIndex);
            }
        }

        private void RecalcularFormulasLinha(DataRowView dataRowView, int rowIndex)
        {
            foreach (var key in _formulas.Keys)
            {
                if (key.rowIndex == rowIndex)
                {
                    string formula = _formulas[key];
                    string formulaBody = formula.Substring(1);

                    var regex = new Regex(@"([a-zA-Z_]+)");
                    var resultFormula = regex.Replace(formulaBody, match =>
                    {
                        var colName = match.Groups[1].Value;
                        if (dataRowView.DataView.Table.Columns.Contains(colName))
                        {
                            var val = dataRowView[colName];
                            return val?.ToString() ?? "0";
                        }
                        return "0";
                    });

                    try
                    {
                        var result = new DataTable().Compute(resultFormula, null);
                        dataRowView[key.columnName] = result;
                    }
                    catch
                    {
                        dataRowView[key.columnName] = "Erro";
                    }
                }
            }
        }

        private void dataGridOrcamento_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dataGridOrcamento.CurrentItem is DataRowView dataRowView)
            {
                int rowIndex = dataGridOrcamento.Items.IndexOf(dataRowView);
                RecalcularFormulasLinha(dataRowView, rowIndex);
            }
        }

        private void Carregar()
        {
            try
            {
                string json = _orcamentoService.CarregarModeloJsonPorOrcamentoId(_orcamentoId, _email);
                if (string.IsNullOrWhiteSpace(json))
                {
                    MessageBox.Show("Nenhum modelo encontrado para este orçamento.");
                    return;
                }

                JObject obj = JObject.Parse(json);
                DataTable tabela = _orcamentoService.ModeloJsonParaDataTable(json);
                dataGridOrcamento.ItemsSource = tabela.DefaultView;

                // Restaura as fórmulas
                _formulas.Clear();
                if (obj["Formulas"] is JObject formulasObj)
                {
                    foreach (var prop in formulasObj.Properties())
                    {
                        var parts = prop.Name.Split(':');
                        if (parts.Length == 2 && int.TryParse(parts[0], out int rowIdx))
                        {
                            _formulas[(rowIdx, parts[1])] = prop.Value.ToString();
                        }
                    }
                }

                // Recalcula todas as fórmulas
                for (int i = 0; i < tabela.Rows.Count; i++)
                {
                    var drv = tabela.DefaultView[i] as DataRowView;
                    RecalcularFormulasLinha(drv, i);
                }

                dataGridOrcamento.CurrentCellChanged += dataGridOrcamento_CurrentCellChanged;
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

                    // Salva também as fórmulas
                    var formulasParaSalvar = new Dictionary<string, string>();
                    foreach (var kv in _formulas)
                    {
                        formulasParaSalvar[$"{kv.Key.rowIndex}:{kv.Key.columnName}"] = kv.Value;
                    }

                    _orcamentoService.SalvarDadosOrcamento(_orcamentoId, tabela, usuarioId, formulasParaSalvar);
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
