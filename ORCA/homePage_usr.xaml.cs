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
        public string _email;

        public homePage_usr(string email, string servidor, string bd, string usr, string senha)
        {
            InitializeComponent();

            _email = email;
            _orcamentoService = new OrcamentoService(servidor, bd, usr, senha);

            CarregarOrcamentosExistentes();
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

        private void CriarOrcamentoVisual(int id, string nome)
        {

            // ===== Card principal =====
            var border = new Border
            {
                Width = 180,
                Height = 100,
                Margin = new Thickness(10),
                CornerRadius = new CornerRadius(12),
                Background = (Brush)new BrushConverter().ConvertFrom("#0B1113"),
                BorderBrush = (Brush)new BrushConverter().ConvertFrom("#3CB86F"),
                BorderThickness = new Thickness(1.5),
                Padding = new Thickness(10)
            };

            // Efeito visual ao passar o mouse
            border.MouseEnter += (s, e) =>
            {
                border.Background = (Brush)new BrushConverter().ConvertFrom("#10181C");
            };
            border.MouseLeave += (s, e) =>
            {
                border.Background = (Brush)new BrushConverter().ConvertFrom("#0B1113");
            };

            // ===== Conteúdo interno =====
            var panel = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            // Nome do modelo
            var nomeText = new TextBlock
            {
                Text = nome,
                FontWeight = FontWeights.Bold,
                FontSize = 14,
                Foreground = (Brush)new BrushConverter().ConvertFrom("#AEE0B4"),
                TextAlignment = TextAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 5, 0, 10)
            };
            panel.Children.Add(nomeText);

            // ===== Painel de botões =====
            var btnPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            var btnEntrar = new Button
            {
                Content = "Entrar",
                Tag = id, // ✅ o botão carrega o ID do orçamento
                Width = 70,
                Height = 28,
                Cursor = Cursors.Hand,
                FontWeight = FontWeights.SemiBold,
                Foreground = (Brush)new BrushConverter().ConvertFrom("#AEE0B4"),
                Background = Brushes.Transparent,
                BorderBrush = (Brush)new BrushConverter().ConvertFrom("#3CB86F"),
                BorderThickness = new Thickness(1.4),
                Margin = new Thickness(0, 0, 10, 0)
            };

            btnEntrar.Click += (s, e) =>
            {
                if ((s as Button)?.Tag is int orcamentoId)
                {
                    var win = new OrcamentoWindow(orcamentoId, _orcamentoService, _email, nome);
                    win.Show(); // ou ShowDialog()
                }
            };

            panel.Children.Add(btnEntrar);

            panel.Children.Add(btnPanel);
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
            bool? resultado = perfilWin.ShowDialog();

            if (resultado == true)
            {
                MessageBox.Show("Perfil atualizado com sucesso! Reinicie o sistema para aplicar as mudanças de login (se alterou e-mail).");
                _email = Sessao.email; // Atualiza variável local também, se necessário

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

        private void btnVoltar_Click(object sender, RoutedEventArgs e)
        {
            // Fecha todas as janelas abertas, menos esta (para evitar erro de coleção modificada)
            foreach (Window janela in Application.Current.Windows)
            {
                if (janela != this)
                    janela.Close();
            }

            // Abre a tela inicial
            MainWindow telaInicial = new MainWindow();
            telaInicial.Show();

            // Fecha a janela atual por último
            this.Close();
        }
    }
}
