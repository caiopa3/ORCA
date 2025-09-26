using ORCA.Services;
using System;
using System.Collections.Generic;
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

        public criar_orca(string email, string servidor, string bd, string usr, string senha)
        {
            InitializeComponent();

            _email = email;
            _modeloService = new ModeloOrcamentoService(servidor, bd, usr, senha);

            // uma linha inicial vazia para o usuário começar
            _linhas.Add(new Dictionary<string, object>());

            // bind do grid
            meuDataGrid.AutoGenerateColumns = false;
            meuDataGrid.ItemsSource = _linhas;
            meuDataGrid.CanUserAddRows = true;

            // quando o usuário adiciona uma nova linha no grid, garantimos o dicionário
            meuDataGrid.RowEditEnding += (s, e) =>
            {
                if (e.EditAction == DataGridEditAction.Commit && e.Row.Item is Dictionary<string, object> dic)
                {
                    foreach (var c in _colunas)
                        if (!dic.ContainsKey(c)) dic[c] = "";
                }
            };
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

                    if (string.IsNullOrWhiteSpace(nomeColuna))
                    {
                        MessageBox.Show("Nome da coluna é obrigatório.");
                        return;
                    }

                    var header = $"{nomeColuna} ({tipo})";

                    // adiciona na lista de colunas                     // adiciona na lista de colunas (usamos o nome 'cru' como chave no dicionário)
(usamos o nome 'cru' como chave no dicionário)
                    if (!_colunas.Contains(nomeColuna))
                         // cria a coluna visualmente
                   _colunas.Add(nomeColuna);

                    // cria a coluna visualmente
                    var novaColuna = new DataGridTextColumn
     System.Windows.Data.               {
                        Header = header,
                        Binding = new System.Windows.Data.Binding($"[{nomeColuna}]")
                        {
                                           }
       };
                  meuDataGrid.Columns.Add(novaColuna);

                                   // garante que todas as linhas tenham essa chave
     // garante que todas as linhas tenham essa chavforeach (var linha in _linhas)
                        if (!linha.ContainsKey(nomeColuna))
               linhms.Refresh();
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
                // pega nome via InputBox (já que não existe txtNomeModelo no XAML)
                string nomeModelo = Microsoft.VisualBasic.Interaction.InputBox(
                    "Digite o nome do modelo:",
                    "Salvar Modelo",
                    "Novo Modelo");

                if (string.IsNullOrWhiteSpace(nomeModelo))
                {
                    MessageBox.Show("Nome do modelo é obrigatório.");
                    return;
                }

                // resolve o id do criador a partir do email
                int usuarioCriadorId = _modeloService.ObterUsuarioIdPorEmail(_email);

                // se o usuário adicionou linhas diretamente no DataGrid, garanta que a fonte (List<Dictionary>) está sincronizada
                // (o DataGrid já está ligado em _linhas; garantir chaves vazias)
                foreach (var linha in _linhas)
                    foreach (var c in _colunas)
                        if (!linha.ContainsKey(c)) linha[c] = "";

                // janela de seleção de usuários (opcional)
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
                    usuariosCompartilhados);

                MessageBox.Show($"Modelo salvo com sucesso! ID = {modeloId}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar modelo: " + ex.Message);
            }
        }
    }
}
