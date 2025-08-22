using ORCA.Services;
using System;
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
            InputBox("Criar Orçamento", "Digite o nome do orçamento:", "Novo Orçamento", nome =>
            {
                if (string.IsNullOrWhiteSpace(nome)) return;

                try
                {
                    int id = _orcamentoService.InserirOrcamento(nome, _email);
                    if (id > 0)
                        CriarOrcamentoVisual(id, nome);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao criar orçamento: " + ex.Message);
                }
            });
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

            // opcional: renomear via clique
            nomeText.MouseLeftButtonDown += (s, e) =>
            {
                if (e.ClickCount == 2)
                {
                    InputBox("Renomear Orçamento", "Novo nome:", nome, novoNome =>
                    {
                        if (!string.IsNullOrWhiteSpace(novoNome))
                        {
                            try
                            {
                                _orcamentoService.AtualizarNome(id, novoNome);
                                nomeText.Text = novoNome;
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Erro ao renomear: " + ex.Message);
                            }
                        }
                    });
                }
            };

            var btnEntrar = new Button
            {
                Content = "Entrar",
                Margin = new Thickness(0, 5, 0, 0),
                Tag = nome
            };

            btnEntrar.Click += (s, e) =>
            {
                MessageBox.Show($"Entrando em: {nome}");
                // TODO: abra a janela do orçamento real
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
    }
}
