using ORCA.Services;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ORCA
{
    public partial class homePage_usr : Window
    {
        private readonly OrcamentoService _orcamentoService;
        private readonly string _email;

        public homePage_usr(string email, string servidor, string bd, string usr, string senha)
        {
            InitializeComponent();

            _email = email;
            _orcamentoService = new OrcamentoService(servidor, bd, usr, senha);

            CarregarOrcamentosExistentes();
        }

        private void CarregarOrcamentosExistentes()
        {
            wrapPanelOrcamentos.Children.Clear();

            try
            {
                var itens = _orcamentoService.ListarPorEmail(_email);
                foreach (var (id, nome) in itens)
                {
                    CriarOrcamentoVisual(id, nome);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar orçamentos: " + ex.Message);
            }
        }

        // CHAME este método num botão da sua tela (ex.: Click="BtnCriarOrcamento_Click")
        private void BtnCriarOrcamento_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 🔹 1. Buscar modelos de orçamento disponíveis para o usuário
                var modelos = _orcamentoService.ListarModelosDisponiveis(_email);

                if (modelos.Count == 0)
                {
                    MessageBox.Show("Nenhum modelo disponível para você. Contate o gestor/administrador.");
                    return;
                }

                // 🔹 2. Abrir janela de seleção de modelo
                SelecionarModeloWindow sel = new SelecionarModeloWindow(modelos);
                if (sel.ShowDialog() == true)
                {
                    var modeloEscolhido = sel.ModeloSelecionado;
                    if (modeloEscolhido == null) return;

                    // 🔹 3. Perguntar nome do novo orçamento
                    InputBox("Criar Orçamento", "Digite o nome do orçamento:", "Novo Orçamento", nome =>
                    {
                        if (string.IsNullOrWhiteSpace(nome)) return;

                        try
                        {
                            // Inserir o orçamento já ligado ao modelo
                            int id = _orcamentoService.InserirOrcamentoComModelo(nome, _email, modeloEscolhido.Id);
                            if (id > 0)
                                CriarOrcamentoVisual(id, nome);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Erro ao criar orçamento: " + ex.Message);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao buscar modelos: " + ex.Message);
            }
        }

        private void CriarOrcamentoVisual(int id, string nome)
        {
            var border = new Border
            {
                Width = 180,
                Height = 80,
                Margin = new Thickness(10),
                Background = Brushes.LightGray,
                CornerRadius = new CornerRadius(5),
                Padding = new Thickness(10)
            };

            var panel = new StackPanel();

            var nomeText = new TextBlock
            {
                Text = nome,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Cursor = Cursors.Hand,
                Tag = id
            };

            var btnEntrar = new Button
            {
                Content = "Entrar",
                Margin = new Thickness(0, 5, 0, 0),
                Tag = id // ✅ o botão carrega o ID do orçamento
            };

            btnEntrar.Click += (s, e) =>
            {
                if ((s as Button)?.Tag is int orcamentoId)
                {
                    var win = new OrcamentoWindow(orcamentoId, _orcamentoService, _email);
                    win.Show(); // ou ShowDialog()
                }
            };

            panel.Children.Add(nomeText);
            panel.Children.Add(btnEntrar);
            border.Child = panel;

            wrapPanelOrcamentos.Children.Add(border);
        }

        private void InputBox(string title, string prompt, string defaultValue, Action<string> onConfirm)
        {
            var inputWindow = new Window
            {
                Title = title,
                Width = 320,
                Height = 160,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize,
                Owner = this
            };

            var panel = new StackPanel { Margin = new Thickness(10) };
            panel.Children.Add(new TextBlock { Text = prompt });

            var input = new TextBox { Text = defaultValue };
            panel.Children.Add(input);

            var ok = new Button { Content = "OK", Margin = new Thickness(0, 10, 0, 0), Width = 60 };
            ok.Click += (_, __) =>
            {
                onConfirm(input.Text.Trim());
                inputWindow.Close();
            };

            panel.Children.Add(ok);
            inputWindow.Content = panel;
            inputWindow.ShowDialog();
        }

        private void BtnPerfil_Click(object sender, RoutedEventArgs e)
        {
            var perfilWin = new PerfilWindow(_email, _orcamentoService);
            if (perfilWin.ShowDialog() == true)
            {
                MessageBox.Show("Reinicie o sistema para aplicar as mudanças de login.");
            }
        }

        private void TextBlock_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            FiltrarOrcamentos();
        }

        private void FiltrarOrcamentos()
        {
            string filtro = txtSearch.Text.Trim();

            wrapPanelOrcamentos.Children.Clear();

            try
            {
                var itens = _orcamentoService.ListarPorEmail(_email);

                if (!string.IsNullOrEmpty(filtro))
                {
                    itens = itens
                        .Where(o => o.Nome.Contains(filtro, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                foreach (var (id, nome) in itens)
                {
                    CriarOrcamentoVisual(id, nome);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao filtrar orçamentos: " + ex.Message);
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filtro = txtSearch.Text.Trim();

            wrapPanelOrcamentos.Children.Clear();

            try
            {
                var itens = _orcamentoService.ListarPorEmail(_email);

                if (!string.IsNullOrEmpty(filtro))
                {
                    itens = itens
                        .Where(o => o.Nome.Contains(filtro, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                foreach (var (id, nome) in itens)
                {
                    CriarOrcamentoVisual(id, nome);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao filtrar orçamentos: " + ex.Message);
            }
        }
    }
}
