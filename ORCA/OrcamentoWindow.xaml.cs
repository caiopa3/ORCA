using ORCA.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace ORCA
{
    public partial class OrcamentoWindow : Window
    {
        private readonly int _orcamentoId;
        private readonly OrcamentoService _orcamentoService;

        // memória local (como no criar_orca)
        private readonly List<string> _colunas = new();
        private readonly List<Dictionary<string, object>> _linhas = new();

        public OrcamentoWindow(int orcamentoId, OrcamentoService orcamentoService)
        {
            InitializeComponent();
            _orcamentoId = orcamentoId;
            _orcamentoService = orcamentoService;

            CarregarModelo();
        }

        private void CarregarModelo()
        {
            try
            {
                string json = _orcamentoService.CarregarModeloJsonPorOrcamentoId(_orcamentoId);
                if (string.IsNullOrWhiteSpace(json))
                {
                    MessageBox.Show("Nenhum modelo encontrado para este orçamento.");
                    return;
                }

                // desserializa JSON
                var modelo = JsonSerializer.Deserialize<ModeloOrcamentoDTO>(json);
                if (modelo == null) return;

                _colunas.Clear();
                _colunas.AddRange(modelo.Colunas);

                _linhas.Clear();
                _linhas.AddRange(modelo.Linhas);

                // monta o grid dinamicamente
                dataGridOrcamento.AutoGenerateColumns = false;
                dataGridOrcamento.Columns.Clear();

                foreach (var col in _colunas)
                {
                    dataGridOrcamento.Columns.Add(new DataGridTextColumn
                    {
                        Header = col,
                        Binding = new System.Windows.Data.Binding($"[{col}]") // correção aqui
                    });
                }


                dataGridOrcamento.ItemsSource = _linhas;
                dataGridOrcamento.CanUserAddRows = true;

                dataGridOrcamento.RowEditEnding += (s, e) =>
                {
                    if (e.EditAction == DataGridEditAction.Commit && e.Row.Item is Dictionary<string, object> dic)
                    {
                        foreach (var c in _colunas)
                            if (!dic.ContainsKey(c)) dic[c] = "";
                    }
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar modelo: " + ex.Message);
            }
        }

        private void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // garante sincronização
                foreach (var linha in _linhas)
                    foreach (var c in _colunas)
                        if (!linha.ContainsKey(c)) linha[c] = "";

                var jsonAtualizado = JsonSerializer.Serialize(new ModeloOrcamentoDTO
                {
                    Colunas = _colunas,
                    Linhas = _linhas
                });

                // salva no banco
                _orcamentoService.SalvarDadosOrcamento(_orcamentoId, jsonAtualizado);

                MessageBox.Show("Orçamento salvo com sucesso!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar orçamento: " + ex.Message);
            }
        }

        private void SalvarModelo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Garante que todas as linhas tenham todas as colunas
                foreach (var linha in _linhas)
                    foreach (var c in _colunas)
                        if (!linha.ContainsKey(c)) linha[c] = "";

                // Serializa para JSON
                var jsonAtualizado = System.Text.Json.JsonSerializer.Serialize(new ModeloOrcamentoDTO
                {
                    Colunas = _colunas,
                    Linhas = _linhas
                });

                // Salva no banco usando OrcamentoService
                _orcamentoService.SalvarDadosOrcamento(_orcamentoId, jsonAtualizado);

                MessageBox.Show("Orçamento salvo com sucesso!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar orçamento: " + ex.Message);
            }
        }

        private void BtnSair_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    // DTO auxiliar
    public class ModeloOrcamentoDTO
    {
        public List<string> Colunas { get; set; } = new();
        public List<Dictionary<string, object>> Linhas { get; set; } = new();
    }
}
