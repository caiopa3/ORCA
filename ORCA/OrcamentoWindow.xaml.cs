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
using Newtonsoft.Json.Linq;

namespace ORCA
{
    public partial class OrcamentoWindow : Window
    {
        private readonly Dictionary<(int rowIndex, string columnName), string> _formulas = new();
        private readonly int _orcamentoId;
        private readonly OrcamentoService _orcamentoService;
        private readonly string _email;
        public string tituloJanela;

        public OrcamentoWindow(int orcamentoId, OrcamentoService orcamentoService, string email, string nome)
        {
            InitializeComponent();

            _orcamentoId = orcamentoId;
            _orcamentoService = orcamentoService;
            _email = email;
            tituloJanela = nome;
            this.Title = $"Orçamento - {tituloJanela}";

            Carregar();

            dataGridOrcamento.CellEditEnding += DataGridOrcamento_CellEditEnding;
            dataGridOrcamento.CurrentCellChanged += dataGridOrcamento_CurrentCellChanged;
            dataGridOrcamento.LoadingRow += DataGridOrcamento_LoadingRow; // <- adiciona numeração dinâmica
        }

        // ======================================
        // NUMERAÇÃO AUTOMÁTICA (coluna #)
        // ======================================

        private void DataGridOrcamento_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (e.Row.Item is DataRowView rowView)
            {
                var table = rowView.DataView?.Table;
                if (table == null) return;

                // garante que a coluna "#" exista
                if (!table.Columns.Contains("#"))
                {
                    table.Columns.Add("#", typeof(int));
                    table.Columns["#"].SetOrdinal(0);
                }

                // atualiza a numeração para todas as linhas
                for (int i = 0; i < table.Rows.Count; i++)
                    table.Rows[i]["#"] = i + 1;
            }
        }

        // ======================================
        // EVENTOS PRINCIPAIS
        // ======================================

        private void DataGridOrcamento_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Row.Item is not DataRowView dataRowView) return;
            if (e.EditingElement is not TextBox editor) return;

            string columnName;
            if (e.Column is DataGridBoundColumn boundCol && boundCol.Binding is System.Windows.Data.Binding binding)
                columnName = binding.Path?.Path ?? (e.Column.Header?.ToString() ?? "");
            else
                columnName = e.Column.Header?.ToString() ?? "";

            string cellValue = editor.Text?.Trim() ?? "";
            int rowIndex = dataGridOrcamento.Items.IndexOf(dataRowView);

            if (cellValue.StartsWith("="))
                _formulas[(rowIndex, columnName)] = cellValue;
            else
                _formulas.Remove((rowIndex, columnName));

            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    if (e.EditAction == DataGridEditAction.Commit)
                        dataGridOrcamento.CommitEdit(DataGridEditingUnit.Row, true);
                }
                catch { }

                RecalcularFormulasLinha(dataRowView, rowIndex);
            }), DispatcherPriority.Background);
        }

        private void dataGridOrcamento_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dataGridOrcamento.CurrentItem is DataRowView dataRowView)
            {
                int rowIndex = dataGridOrcamento.Items.IndexOf(dataRowView);
                RecalcularFormulasLinha(dataRowView, rowIndex);
            }
        }

        // ======================================
        // REGRAS DE CÁLCULO (SUPORTE [n]Coluna)
        // ======================================

        private void RecalcularFormulasLinha(DataRowView dataRowView, int rowIndex)
        {
            if (dataRowView == null || dataRowView.DataView == null) return;

            var table = dataRowView.DataView.Table;
            if (table == null) return;

            var colunas = table.Columns.Cast<DataColumn>()
                              .Select(c => c.ColumnName)
                              .OrderByDescending(s => s.Length)
                              .ToList();

            var formulasDaLinha = _formulas.Where(k => k.Key.rowIndex == rowIndex).ToList();

            foreach (var kv in formulasDaLinha)
            {
                var chave = kv.Key;
                string formula = kv.Value ?? "";
                if (formula.Length == 0) continue;
                string expr = formula.Substring(1);

                // 1️⃣ Referências entre linhas [n]Coluna
                expr = Regex.Replace(expr, @"\[(\d+)\]([\p{L}\p{N}_\s]+)", match =>
                {
                    int refRow = int.Parse(match.Groups[1].Value) - 1;
                    string colRef = match.Groups[2].Value.Trim();

                    if (refRow < 0 || refRow >= table.Rows.Count)
                        return "0";
                    if (!table.Columns.Contains(colRef))
                        return "0";

                    var val = table.Rows[refRow][colRef]?.ToString()?.Trim() ?? "0";

                    if (double.TryParse(val, NumberStyles.Any, CultureInfo.InvariantCulture, out double dInv))
                        return dInv.ToString(CultureInfo.InvariantCulture);
                    if (double.TryParse(val, NumberStyles.Any, CultureInfo.CurrentCulture, out double dCur))
                        return dCur.ToString(CultureInfo.InvariantCulture);

                    return "0";
                }, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

                // 2️⃣ Substitui colunas da própria linha
                foreach (var col in colunas)
                {
                    string pattern = $@"(?<![\p{{L}}\p{{N}}_]){Regex.Escape(col)}(?![\p{{L}}\p{{N}}_])";

                    expr = Regex.Replace(expr, pattern, m =>
                    {
                        string matched = m.Value;
                        var dataCol = table.Columns.Cast<DataColumn>()
                                      .FirstOrDefault(c => string.Equals(c.ColumnName, matched, StringComparison.OrdinalIgnoreCase));

                        if (dataCol == null) return "0";

                        var valObj = dataRowView[dataCol.ColumnName];
                        var valStr = valObj?.ToString()?.Trim() ?? "";

                        if (string.IsNullOrEmpty(valStr))
                            return "0";

                        if (double.TryParse(valStr, NumberStyles.Any, CultureInfo.CurrentCulture, out double d) ||
                            double.TryParse(valStr, NumberStyles.Any, CultureInfo.InvariantCulture, out d))
                        {
                            return d.ToString(CultureInfo.InvariantCulture);
                        }

                        return "0";
                    }, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
                }

                // 3️⃣ Avalia a expressão
                try
                {
                    var computed = new DataTable().Compute(expr, null);
                    if (table.Columns.Contains(chave.columnName))
                        dataRowView[chave.columnName] = computed;
                    else
                        dataRowView[table.Columns.Cast<DataColumn>()
                            .FirstOrDefault(c => string.Equals(c.ColumnName, chave.columnName, StringComparison.OrdinalIgnoreCase))?.ColumnName ?? chave.columnName] = computed;
                }
                catch
                {
                    try
                    {
                        if (table.Columns.Contains(chave.columnName))
                            dataRowView[chave.columnName] = "Erro";
                    }
                    catch { }
                }
            }
        }

        // ======================================
        // CARREGAMENTO E SALVAR
        // ======================================

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

                // adiciona coluna "#" no início
                if (!tabela.Columns.Contains("#"))
                {
                    tabela.Columns.Add("#", typeof(int));
                    tabela.Columns["#"].SetOrdinal(0);
                }

                // numera as linhas
                for (int i = 0; i < tabela.Rows.Count; i++)
                    tabela.Rows[i]["#"] = i + 1;

                dataGridOrcamento.ItemsSource = tabela.DefaultView;

                // restaura fórmulas
                _formulas.Clear();
                if (obj["Formulas"] is JObject formulasObj)
                {
                    foreach (var prop in formulasObj.Properties())
                    {
                        var parts = prop.Name.Split(':');
                        if (parts.Length == 2 && int.TryParse(parts[0], out int rowIdx))
                            _formulas[(rowIdx, parts[1])] = prop.Value.ToString();
                    }
                }

                // recalcula tudo
                for (int i = 0; i < tabela.Rows.Count; i++)
                    if (tabela.DefaultView[i] is DataRowView drv)
                        RecalcularFormulasLinha(drv, i);
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

                    if (tabela.Columns.Contains("#"))
                        tabela.Columns.Remove("#");

                    int usuarioId = _orcamentoService.ObterUsuarioIdPorEmail(_email);

                    var formulasParaSalvar = new Dictionary<string, string>();
                    foreach (var kv in _formulas)
                        formulasParaSalvar[$"{kv.Key.rowIndex}:{kv.Key.columnName}"] = kv.Value;

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
                var dt = new DataTable("Orcamento");

                foreach (var col in dataGridOrcamento.Columns)
                    dt.Columns.Add(col.Header.ToString());

                foreach (var item in dataGridOrcamento.Items)
                {
                    if (item is DataRowView rowView)
                        dt.Rows.Add(rowView.Row.ItemArray);
                }

                var pdfWin = new gerar_pdf(dt, tituloJanela);
                pdfWin.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao exportar PDF: " + ex.Message);
            }
        }

        private void btnVoltar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
