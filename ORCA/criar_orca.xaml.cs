using Newtonsoft.Json.Linq;
using ORCA.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace ORCA
{
    public partial class criar_orca : Window
    {
        private readonly ModeloOrcamentoService _modeloService;
        private readonly string _email;

        private readonly string _servidor;
        private readonly string _bd;
        private readonly string _usr;
        private readonly string _senha;

        // dados em memória
        private readonly List<string> _colunas = new();
        private readonly List<Dictionary<string, object>> _linhas = new();

        // valores fixos por coluna (persistidos)
        private readonly Dictionary<string, object> _valoresFixos = new(StringComparer.OrdinalIgnoreCase);

        // flags para modo de edição
        private bool _modoEdicao = false;
        private int _modeloIdEmEdicao = 0;

        public criar_orca(string email, string servidor, string bd, string usr, string senha)
        {
            InitializeComponent();

            // armazenando os parâmetros recebidos
            _email = email;
            _servidor = servidor;
            _bd = bd;
            _usr = usr;
            _senha = senha;
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

        // CONSTRUTOR para edição: recebe modeloId, reaproveita o construtor base
        public criar_orca(string email, string servidor, string bd, string usr, string senha, int modeloId)
            : this(email, servidor, bd, usr, senha)
        {
            _modoEdicao = true;
            _modeloIdEmEdicao = modeloId;
            CarregarModeloParaEdicao(modeloId);
        }

        private void MeuDataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            // Normalmente o DataGrid criará um Dictionary<string, object> para nós
            if (e.NewItem is Dictionary<string, object> novoDic)
            {
                foreach (var c in _colunas)
                {
                    if (!novoDic.ContainsKey(c))
                        novoDic[c] = _valoresFixos.TryGetValue(c, out var vf) ? vf : "";
                }
            }
        }

        // Ao terminar de editar uma linha garantimos que todas as chaves existam e respeitamos valores fixos
        private void MeuDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit && e.Row.Item is Dictionary<string, object> dic)
            {
                foreach (var c in _colunas)
                {
                    if (!dic.ContainsKey(c))
                        dic[c] = _valoresFixos.ContainsKey(c) ? _valoresFixos[c] : "";
                    else if (dic[c] == null)
                        dic[c] = _valoresFixos.ContainsKey(c) ? _valoresFixos[c] : "";
                }
            }
        }

        // Carrega JSON do modelo e popula colunas, linhas e fixed values
        private void CarregarModeloParaEdicao(int modeloId)
        {
            try
            {
                string json = _modeloService.ObterDadosJsonDoModelo(modeloId);
                if (string.IsNullOrWhiteSpace(json)) return;

                var obj = JObject.Parse(json);

                // limpa estruturas antigas
                _colunas.Clear();
                _linhas.Clear();
                _valoresFixos.Clear();
                meuDataGrid.Columns.Clear();

                // Colunas (mantém tipo no header se existir)
                var colunas = obj["Colunas"]?.Select(c => c.ToString()).ToList() ?? new List<string>();

                // Linhas JArray
                var linhasJ = obj["Linhas"] as JArray ?? new JArray();

                // FixedValues (pode conter chaves com (Tipo) também)
                if (obj["FixedValues"] is JObject fvObj)
                {
                    foreach (var prop in fvObj.Properties())
                    {
                        var key = RemoverTipo(prop.Name);
                        var token = prop.Value;
                        // tenta interpretar numero / boolean / string
                        if (token != null && (token.Type == JTokenType.Float || token.Type == JTokenType.Integer))
                        {
                            _valoresFixos[key] = token.ToObject<double>();
                        }
                        else if (token != null && token.Type == JTokenType.Boolean)
                        {
                            _valoresFixos[key] = token.ToObject<bool>();
                        }
                        else
                        {
                            var s = token?.ToString() ?? "";
                            if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out double dinv))
                                _valoresFixos[key] = dinv;
                            else if (double.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out double dcur))
                                _valoresFixos[key] = dcur;
                            else
                                _valoresFixos[key] = s;
                        }
                    }
                }

                // Se Colunas vazio, Tenta inferir a partir da primeira linha
                if (!colunas.Any() && linhasJ.Count > 0 && linhasJ[0] is JObject firstRow)
                {
                    colunas = firstRow.Properties().Select(p => RemoverTipo(p.Name)).ToList();
                }

                // Criar colunas visuais e registrar na lista de colunas
                foreach (var rawCol in colunas)
                {
                    var clean = RemoverTipo(rawCol);
                    _colunas.Add(clean);

                    var safeKey = clean.Replace("'", "\\'");
                    var bindingPath = $"['{safeKey}']";

                    var binding = new Binding(bindingPath)
                    {
                        Mode = BindingMode.TwoWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.LostFocus
                    };

                    var novaCol = new DataGridTextColumn
                    {
                        Header = CriarHeaderComRemover(rawCol, clean),
                        Binding = binding
                    };
                    meuDataGrid.Columns.Add(novaCol);
                }

                // Preenche linhas
                foreach (var item in linhasJ.OfType<JObject>())
                {
                    var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                    foreach (var prop in item.Properties())
                    {
                        var colName = RemoverTipo(prop.Name);
                        dict[colName] = prop.Value?.ToString() ?? "";
                    }

                    // garante chaves faltantes e aplica valores fixos
                    foreach (var c in _colunas)
                    {
                        if (!dict.ContainsKey(c))
                            dict[c] = _valoresFixos.TryGetValue(c, out var vf) ? vf : "";
                    }

                    _linhas.Add(dict);
                }

                // se nenhuma linha, adiciona uma vazia (com valores fixos)
                if (_linhas.Count == 0)
                    _linhas.Add(new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase));

                meuDataGrid.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar modelo para edição: " + ex.Message);
            }
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
                        Header = CriarHeaderComRemover(header, nomeColuna),
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

        // Botão Salvar modelo: cria ou atualiza. Retorna DialogResult = true em sucesso.
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
                    _modoEdicao ? "Editar Modelo" : "Novo Modelo");

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
                    usuariosCompartilhados.AddRange(selecao.UsuariosSelecionados);

                if (_modoEdicao)
                {
                    // atualiza modelo existente
                    _modeloService.AtualizarModeloOrcamento(
                        _modeloIdEmEdicao,
                        nomeModelo,
                        _colunas.ToList(),
                        _linhas.ToList(),
                        usuariosCompartilhados,
                        _valoresFixos);

                    MessageBox.Show("Modelo atualizado com sucesso!");
                }
                else
                {
                    int modeloId = _modeloService.CriarModeloOrcamento(
                        nomeModelo,
                        usuarioCriadorId,
                        _colunas.ToList(),
                        _linhas.ToList(),
                        usuariosCompartilhados,
                        _valoresFixos);

                    MessageBox.Show($"Modelo salvo com sucesso! ID = {modeloId}");
                }

                // sinaliza sucesso para quem chamou ShowDialog()
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar modelo: " + ex.Message);
            }
        }

        // utilidade para remover "(Tipo)" dos nomes quando necessário
        private string RemoverTipo(string coluna)
        {
            try
            {
                return System.Text.RegularExpressions.Regex.Replace(coluna, @"\s*\(.*?\)", "");
            }
            catch
            {
                return coluna;
            }
        }

        // Cria um header visual com texto + botão de remover.
        // displayHeader: o texto que aparecerá no header (ex: "Produto (Texto)")
        // keyName: a chave real/limpa usada no binding (ex: "Produto")
        private FrameworkElement CriarHeaderComRemover(string displayHeader, string keyName)
        {
            var sp = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center
            };

            var txt = new TextBlock
            {
                Text = displayHeader,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 6, 0)
            };

            var btnRem = new Button
            {
                Content = "✖",
                Width = 20,
                Height = 20,
                FontSize = 12,
                Padding = new Thickness(0),
                Margin = new Thickness(0),
                VerticalAlignment = VerticalAlignment.Center,
                Cursor = Cursors.Hand,
                ToolTip = "Remover coluna"
            };

            btnRem.BorderThickness = new Thickness(0);
            btnRem.Background = Brushes.Transparent;
            btnRem.Foreground = Brushes.DarkRed;

            btnRem.Click += (s, e) =>
            {
                var res = MessageBox.Show($"Remover a coluna '{displayHeader}' ?\nDados desta coluna serão perdidos.", "Confirmar remoção", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (res == MessageBoxResult.Yes)
                    RemoverColuna(keyName);
            };

            sp.Children.Add(txt);
            sp.Children.Add(btnRem);
            return sp;
        }

        // Remove a coluna (limpa _colunas, _valoresFixos, chaves nas linhas e coluna visual)
        // Remove a coluna (limpa _colunas, _valoresFixos, chaves nas linhas e coluna visual)
        private void RemoverColuna(string nomeColuna)
        {
            if (string.IsNullOrWhiteSpace(nomeColuna)) return;

            // Remove da lista de colunas
            _colunas.RemoveAll(c => string.Equals(c, nomeColuna, StringComparison.OrdinalIgnoreCase));

            // Remove valor fixo
            if (_valoresFixos.ContainsKey(nomeColuna))
                _valoresFixos.Remove(nomeColuna);

            // Remove chave de todas as linhas
            foreach (var linha in _linhas)
            {
                if (linha.ContainsKey(nomeColuna))
                    linha.Remove(nomeColuna);
            }

            // Remove coluna visual no DataGrid (procura por coluna cujo binding indexer contenha o nome
            // ou cujo header contenha o nome)
            DataGridColumn toRemove = null;
            foreach (var col in meuDataGrid.Columns.OfType<DataGridTextColumn>())
            {
                // tenta extrair texto do header de forma segura:
                string headerText = HeaderToString(col.Header);

                // tenta obter o caminho de binding (ex: ['Produto'])
                string bindingPath = "";
                if (col.Binding is Binding b && b.Path != null)
                    bindingPath = b.Path.Path ?? "";

                if (bindingPath.IndexOf($"['{nomeColuna}']", StringComparison.OrdinalIgnoreCase) >= 0
                    || bindingPath.IndexOf($"[\"{nomeColuna}\"]", StringComparison.OrdinalIgnoreCase) >= 0
                    || headerText.IndexOf(nomeColuna, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    toRemove = col;
                    break;
                }
            }

            if (toRemove != null)
                meuDataGrid.Columns.Remove(toRemove);

            meuDataGrid.Items.Refresh();
        }

        // Helper que extrai um texto legível do Header (tratando StackPanel, TextBlock, string...)
        // mantém seguro contra casts inválidos.
        private string HeaderToString(object header)
        {
            if (header == null) return "";

            // se é o StackPanel que criamos (ou outro Panel), percorre e tenta pegar o primeiro TextBlock
            if (header is Panel panel)
            {
                var tb = panel.Children.OfType<TextBlock>().FirstOrDefault();
                if (tb != null) return tb.Text ?? "";
            }

            // se é um FrameworkElement (ex.: ContentControl) que contém um TextBlock interno
            if (header is FrameworkElement fe)
            {
                // tenta procurar recursivamente um TextBlock (seguro)
                var tb = FindChildTextBlock(fe);
                if (tb != null) return tb.Text ?? "";
            }

            // se é string ou outro objeto simples
            return header.ToString() ?? "";
        }

        // procura recursivamente um TextBlock dentro da árvore visual do FrameworkElement
        private TextBlock FindChildTextBlock(FrameworkElement root)
        {
            if (root == null) return null;

            // use VisualTreeHelper para procurar filhos
            int count = VisualTreeHelper.GetChildrenCount(root);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(root, i);

                if (child is TextBlock tb) return tb;
                if (child is FrameworkElement feChild)
                {
                    var found = FindChildTextBlock(feChild);
                    if (found != null) return found;
                }
            }

            return null;
        }


        private void btnVoltar_Click(object sender, RoutedEventArgs e)
        {
            homePage_adm homePage_Adm = new homePage_adm(_email, _servidor, _bd, _usr, _senha);
            homePage_Adm.Show();
            this.Close();
        }
    }
}
