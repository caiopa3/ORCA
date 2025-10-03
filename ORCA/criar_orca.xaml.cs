using ORCA.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ORCA
{
    public partial class criar_orca : Window
    {
        private readonly ModeloOrcamentoService _modeloService;
        private readonly string _email;

        // dados em memória
        private readonly List<string> _colunas = new();
        private readonly List<Dictionary<string, object>> _linhas = new();

        // valores fixos por coluna (persistidos)
        private readonly Dictionary<string, object> _valoresFixos = new(StringComparer.OrdinalIgnoreCase);

        public criar_orca(string email, string servidor, string bd, string usr, string senha)
        {
            InitializeComponent();

            _email = email;
            _modeloService = new ModeloOrcamentoService(servidor, bd, usr, senha);

            // uma linha inicial vazia
            _linhas.Add(new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase));

            // configura dataGrid
            meuDataGrid.AutoGenerateColumns = false;
            meuDataGrid.ItemsSource = _linhas;
            meuDataGrid.CanUserAddRows = true;

            // garante chaves ao finalizar edição de linha
            meuDataGrid.RowEditEnding += (s, e) =>
            {
                if (e.EditAction == DataGridEditAction.Commit && e.Row.Item is Dictionary<string, object> dic)
                {
                    foreach (var c in _colunas)
                        if (!dic.ContainsKey(c))
                            dic[c] = _valoresFixos.ContainsKey(c) ? _valoresFixos[c] : "";
                }
            };

            // inicializa novos itens criados pelo DataGrid (preenche dicionário com valores fixos)
            meuDataGrid.InitializingNewItem += MeuDataGrid_InitializingNewItem;
        }

        private void MeuDataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            // Normalmente o DataGrid criará um Dictionary<string, object> para nós (porque ItemsSource é List<Dictionary<...>>)
            if (e.NewItem is Dictionary<string, object> novoDic)
            {
                foreach (var c in _colunas)
                {
                    if (!novoDic.ContainsKey(c))
                        novoDic[c] = _valoresFixos.TryGetValue(c, out var vf) ? vf : "";
                }
            }
            // Não tente atribuir e.NewItem (é somente leitura). Se por acaso o DataGrid criar outro tipo (inusual),
            // podemos apenas confiar que o usuário não verá comportamento estranho — na prática isso não acontece
            // quando ItemsSource é List<Dictionary<string,object>>.
        }

        private void AdicionarColuna_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dlg = new NovaColunaWindow();
                if (dlg.ShowDialog() == true)
                {
                    var nomeColuna = dlg.NomeColuna?.Trim();
                    var tipo = string.IsNullOrWhiteSpace(dlg.TipoDado) ? "Texto" : dlg.TipoDado.Trim();
                    var valorFixoTexto = dlg.ValorFixo; // pode ser null

                    if (string.IsNullOrWhiteSpace(nomeColuna))
                    {
                        MessageBox.Show("Nome da coluna é obrigatório.");
                        return;
                    }

                    var header = $"{nomeColuna} ({tipo})";

                    if (!_colunas.Contains(nomeColuna))
                        _colunas.Add(nomeColuna);

                    // converte valor fixo conforme tipo
                    object valorFixo = null;
                    if (!string.IsNullOrWhiteSpace(valorFixoTexto))
                    {
                        if (tipo.Equals("Número", StringComparison.OrdinalIgnoreCase))
                        {
                            var s = valorFixoTexto.Replace(',', '.');
                            if (double.TryParse(s, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out var dv))
                                valorFixo = dv;
                            else if (double.TryParse(valorFixoTexto, System.Globalization.NumberStyles.Any, CultureInfo.CurrentCulture, out dv))
                                valorFixo = dv;
                            else
                                valorFixo = valorFixoTexto;
                        }
                        else if (tipo.Equals("Booleano", StringComparison.OrdinalIgnoreCase))
                        {
                            if (bool.TryParse(valorFixoTexto, out var bv)) valorFixo = bv;
                            else valorFixo = valorFixoTexto;
                        }
                        else
                        {
                            valorFixo = valorFixoTexto;
                        }

                        _valoresFixos[nomeColuna] = valorFixo;
                    }

                    // cria coluna com binding indexer (aspas simples no indexer para suportar espaços)
                    var safeKey = nomeColuna.Replace("'", "\\'");
                    var bindingPath = $"['{safeKey}']";

                    var binding = new Binding(bindingPath)
                    {
                        Mode = BindingMode.TwoWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.LostFocus
                    };

                    var novaColuna = new DataGridTextColumn
                    {
                        Header = header,
                        Binding = binding
                    };
                    meuDataGrid.Columns.Add(novaColuna);

                    // aplica valor fixo nas linhas já existentes
                    foreach (var linha in _linhas)
                    {
                        if (!linha.ContainsKey(nomeColuna))
                            linha[nomeColuna] = valorFixo ?? "";
                    }

                    meuDataGrid.Items.Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao adicionar coluna: " + ex.Message);
            }
        }

        private void SalvarModelo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // força commit de edições pendentes
                meuDataGrid.CommitEdit(DataGridEditingUnit.Cell, true);
                meuDataGrid.CommitEdit(DataGridEditingUnit.Row, true);

                string nomeModelo = Microsoft.VisualBasic.Interaction.InputBox(
                    "Digite o nome do modelo:",
                    "Salvar Modelo",
                    "Novo Modelo");

                if (string.IsNullOrWhiteSpace(nomeModelo))
                {
                    MessageBox.Show("Nome do modelo é obrigatório.");
                    return;
                }

                int usuarioCriadorId = _modeloService.ObterUsuarioIdPorEmail(_email);
                if (usuarioCriadorId <= 0)
                {
                    MessageBox.Show("Usuário inválido. Não foi possível identificar o criador.");
                    return;
                }

                // garante que cada linha tenha todas as colunas (e valores fixos)
                foreach (var linha in _linhas)
                {
                    foreach (var c in _colunas)
                    {
                        if (!linha.ContainsKey(c))
                            linha[c] = _valoresFixos.ContainsKey(c) ? _valoresFixos[c] : "";
                        else if (linha[c] == null)
                            linha[c] = _valoresFixos.ContainsKey(c) ? _valoresFixos[c] : "";
                    }
                }

                // seleção de usuários (opcional)
                var selecao = new SelecionarUsuariosWindow();
                var usuariosCompartilhados = new List<int>();
                if (selecao.ShowDialog() == true)
                {
                    usuariosCompartilhados.AddRange(selecao.UsuariosSelecionados);
                }

                int modeloId = _modeloService.CriarModeloOrcamento(
                    nomeModelo,
                    usuarioCriadorId,
                    _colunas.ToList(),
                    _linhas.ToList(),
                    usuariosCompartilhados,
                    _valoresFixos);

                MessageBox.Show($"Modelo salvo com sucesso! ID = {modeloId}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar modelo: " + ex.Message);
            }
        }
    }
}
