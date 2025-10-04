using ORCA.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ORCA
{
    public partial class OrcamentoWindow : Window
    {
        // guarda fórmulas por (linha, nomeDaColuna)
        private readonly Dictionary<(int rowIndex, string columnName), string> _formulas = new();

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
            dataGridOrcamento.CurrentCellChanged += dataGridOrcamento_CurrentCellChanged;
        }

        private void DataGridOrcamento_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            // pega DataRowView e editor
            if (e.Row.Item is not DataRowView dataRowView) return;
            if (e.EditingElement is not TextBox editor) return;

            // tenta obter o "nome real" da coluna a partir do binding (se existir)
            string columnName;
            if (e.Column is DataGridBoundColumn boundCol && boundCol.Binding is System.Windows.Data.Binding binding)
                columnName = binding.Path?.Path ?? (e.Column.Header?.ToString() ?? "");
            else
                columnName = e.Column.Header?.ToString() ?? "";

            string cellValue = editor.Text?.Trim() ?? "";
            int rowIndex = dataGridOrcamento.Items.IndexOf(dataRowView);

            if (cellValue.StartsWith("="))
            {
                // salva a fórmula (ex: "= Valor Fixo + (Valor Variável * Quantidade)")
                _formulas[(rowIndex, columnName)] = cellValue;
            }
            else
            {
                // se não for fórmula, remove entrada (caso exista)
                _formulas.Remove((rowIndex, columnName));
            }

            // Recalcula **após** o commit acontecer no DataGrid (usando BeginInvoke)
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    // commit da edição para garantir que o DataRowView já contenha o valor editado
                    if (e.EditAction == DataGridEditAction.Commit)
                        dataGridOrcamento.CommitEdit(DataGridEditingUnit.Row, true);
                }
                catch
                {
                    // se o commit falhar, apenas continue (não é crítico)
                }

                RecalcularFormulasLinha(dataRowView, rowIndex);
            }), DispatcherPriority.Background);
        }

        private void RecalcularFormulasLinha(DataRowView dataRowView, int rowIndex)
        {
            if (dataRowView == null || dataRowView.DataView == null) return;

            var table = dataRowView.DataView.Table;
            if (table == null) return;

            // lista de colunas (ordenada por comprimento decrescente para evitar substituições parciais,
            // ex: "Valor" antes de "Valor Fixo")
            var colunas = table.Columns.Cast<DataColumn>()
                              .Select(c => c.ColumnName)
                              .OrderByDescending(s => s.Length)
                              .ToList();

            // busca as fórmulas que pertencem àquela linha
            var formulasDaLinha = _formulas.Where(k => k.Key.rowIndex == rowIndex).ToList();
            foreach (var kv in formulasDaLinha)
            {
                var chave = kv.Key;
                string formula = kv.Value ?? "";
                if (formula.Length == 0) continue;
                string formulaBody = formula.Substring(1); // remove '=' inicial

                string expr = formulaBody;

                // Para cada coluna, substitui ocorrências por valor numérico (invariant)
                foreach (var col in colunas)
                {
                    // pattern: garante que não faça match no meio de outra palavra (usa lookarounds)
                    string pattern = $@"(?<![\p{{L}}\p{{N}}_]){Regex.Escape(col)}(?![\p{{L}}\p{{N}}_])";

                    expr = Regex.Replace(expr, pattern, m =>
                    {
                        // o texto que apareceu na fórmula (pode ter case diferente)
                        string matched = m.Value;

                        // encontra a coluna real (case-insensitive)
                        var dataCol = table.Columns.Cast<DataColumn>()
                                      .FirstOrDefault(c => string.Equals(c.ColumnName, matched, StringComparison.OrdinalIgnoreCase));

                        if (dataCol == null) return "0";

                        var valObj = dataRowView[dataCol.ColumnName];
                        var valStr = valObj?.ToString()?.Trim() ?? "";

                        if (string.IsNullOrEmpty(valStr))
                            return "0";

                        // tenta converter para double (aceitando formatos com vírgula ou ponto)
                        if (double.TryParse(valStr, NumberStyles.Any, CultureInfo.CurrentCulture, out double d) ||
                            double.TryParse(valStr, NumberStyles.Any, CultureInfo.InvariantCulture, out d))
                        {
                            // retorna sempre com ponto decimal (InvariantCulture) para a Evaluate
                            return d.ToString(CultureInfo.InvariantCulture);
                        }

                        // se não for número, retorna 0 para evitar exceções
                        return "0";
                    }, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
                }

                // Agora expr deve conter apenas números, operadores e parênteses
                try
                {
                    var computed = new DataTable().Compute(expr, null);
                    // escreve resultado na célula (usa a coluna real da chave)
                    if (table.Columns.Contains(chave.columnName))
                        dataRowView[chave.columnName] = computed;
                    else
                        // se por algum motivo a coluna da chave não existir (inconsistência), tenta localizar case-insensitive
                        dataRowView[table.Columns.Cast<DataColumn>()
                                        .FirstOrDefault(c => string.Equals(c.ColumnName, chave.columnName, StringComparison.OrdinalIgnoreCase))?.ColumnName ?? chave.columnName] = computed;
                }
                catch
                {
                    // Se der erro na compute, coloca "Erro"
                    try
                    {
                        if (table.Columns.Contains(chave.columnName))
                            dataRowView[chave.columnName] = "Erro";
                    }
                    catch { /* swallow */ }
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

                // Restaura fórmulas salvas (se houver)
                _formulas.Clear();
                if (obj["Formulas"] is JObject formulasObj)
                {
                    foreach (var prop in formulasObj.Properties())
                    {
                        var parts = prop.Name.Split(':');
                        if (parts.Length == 2 && int.TryParse(parts[0], out int rowIdx))
                        {
                            // parts[1] deve ser o nome da coluna (binding path) usado ao salvar
                            _formulas[(rowIdx, parts[1])] = prop.Value.ToString();
                        }
                    }
                }

                // Recalcula todas as fórmulas existentes
                for (int i = 0; i < tabela.Rows.Count; i++)
                {
                    if (tabela.DefaultView[i] is DataRowView drv)
                        RecalcularFormulasLinha(drv, i);
                }
            }
            catch (Exception ex)
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

                    // Salva também as fórmulas (chave: "rowIndex:columnName")
                    var formulasParaSalvar = new Dictionary<string, string>();
                    foreach (var kv in _formulas)
                        formulasParaSalvar[$"{kv.Key.rowIndex}:{kv.Key.columnName}"] = kv.Value;

                    // CHAME o método do service que aceite as fórmulas (adapte se necessário)
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

        private void BtnExportar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 🔹 Converter o DataGrid em um DataTable
                var dt = new DataTable("Orcamento");

                foreach (var col in dataGridOrcamento.Columns)
                {
                    dt.Columns.Add(col.Header.ToString());
                }

                foreach (var item in dataGridOrcamento.Items)
                {
                    if (item is System.Data.DataRowView rowView)
                    {
                        dt.Rows.Add(rowView.Row.ItemArray);
                    }
                    else if (item != null)
                    {
                        // caso o DataGrid esteja ligado a objetos
                        var props = item.GetType().GetProperties();
                        var values = props.Select(p => p.GetValue(item, null)).ToArray();
                        dt.Rows.Add(values);
                    }
                }

                // 🔹 Abrir a tela de edição do PDF (PdfContentWindow)
                var pdfWin = new gerar_pdf(dt);
                pdfWin.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao exportar PDF: " + ex.Message);
            }
        }
    }
}
