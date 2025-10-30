using ORCA.Services;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ORCA
{
    public partial class homePage_adm : Window
    {
        public string servidor = "";
        public string bd = "";
        public string usr = "";
        public string senha = "";
        public string email = "";

        private readonly OrcamentoService _orcamentoService;
        private readonly ModeloOrcamentoService _modeloService;
        private int _usuarioId = 0;

        public homePage_adm(string e, string s, string b, string u, string se)
        {
            InitializeComponent();

            email = e;
            servidor = s;
            bd = b;
            usr = u;
            senha = se;

            _orcamentoService = new OrcamentoService(servidor, bd, usr, senha);
            _modeloService = new ModeloOrcamentoService(servidor, bd, usr, senha);

            // obtém id do usuario logado (criador)
            _usuarioId = _modeloService.ObterUsuarioIdPorEmail(email);

            CarregarModelosCriados();
        }

        private void CarregarModelosCriados()
        {
            panelModelos.Children.Clear();

            try
            {
                var modelos = _modeloService.ListarModelosPorCriadorId(_usuarioId);
                foreach (var (id, nome) in modelos)
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
                    var stack = new StackPanel
                    {
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center
                    };

                    // Nome do modelo
                    var title = new TextBlock
                    {
                        Text = nome,
                        FontWeight = FontWeights.Bold,
                        FontSize = 14,
                        Foreground = (Brush)new BrushConverter().ConvertFrom("#AEE0B4"),
                        TextAlignment = TextAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(0, 5, 0, 10)
                    };
                    stack.Children.Add(title);

                    // ===== Painel de botões =====
                    var btnPanel = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Center
                    };

                    // ==== Botão Editar ====
                    var btnEditar = new Button
                    {
                        Content = "Editar",
                        Tag = id,
                        Width = 100,
                        Height = 28,
                        Cursor = Cursors.Hand,
                        FontWeight = FontWeights.SemiBold,
                        Foreground = (Brush)new BrushConverter().ConvertFrom("#AEE0B4"),
                        Background = Brushes.Transparent,
                        BorderBrush = (Brush)new BrushConverter().ConvertFrom("#3CB86F"),
                        BorderThickness = new Thickness(1.4),
                        Margin = new Thickness(0, 0, 10, 0)
                    };

                    btnEditar.MouseEnter += (s, e) =>
                    {
                        btnEditar.Background = (Brush)new BrushConverter().ConvertFrom("#3CB86F");
                        btnEditar.Foreground = Brushes.White;
                    };
                    btnEditar.MouseLeave += (s, e) =>
                    {
                        btnEditar.Background = Brushes.Transparent;
                        btnEditar.Foreground = (Brush)new BrushConverter().ConvertFrom("#AEE0B4");
                    };

                    btnEditar.Click += (s, e) =>
                    {
                        int modeloId = (int)(s as Button).Tag;
                        var win = new criar_orca(email, servidor, bd, usr, senha, modeloId);
                        bool? ok = win.ShowDialog();
                        CarregarModelosCriados();
                    };

                    // ==== Botão Excluir ====
                    var btnExcluir = new Button
                    {
                        Content = "Excluir",
                        Tag = id,
                        Width = 70,
                        Height = 28,
                        Cursor = Cursors.Hand,
                        FontWeight = FontWeights.SemiBold,
                        Foreground = (Brush)new BrushConverter().ConvertFrom("#A9C7E8"),
                        Background = Brushes.Transparent,
                        BorderBrush = (Brush)new BrushConverter().ConvertFrom("#007ACC"),
                        BorderThickness = new Thickness(1.4)
                        
                    };

                    btnExcluir.MouseEnter += (s, e) =>
                    {
                        btnExcluir.Background = (Brush)new BrushConverter().ConvertFrom("#007ACC");
                        btnExcluir.Foreground = Brushes.White;
                    };
                    btnExcluir.MouseLeave += (s, e) =>
                    {
                        btnExcluir.Background = Brushes.Transparent;
                        btnExcluir.Foreground = (Brush)new BrushConverter().ConvertFrom("#A9C7E8");
                    };

                    btnExcluir.Click += (s, e) =>
                    {
                        if (MessageBox.Show("Deseja realmente excluir este modelo?", "Confirmar", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            int modeloId = (int)(s as Button).Tag;
                            try
                            {
                                _modeloService.ExcluirModelo(modeloId);
                                CarregarModelosCriados();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Erro ao excluir: " + ex.Message);
                            }
                        }
                    };

                    // adiciona botões ao painel
                    btnPanel.Children.Add(btnEditar);
                    btnPanel.Children.Add(btnExcluir);

                    // adiciona ao card
                    stack.Children.Add(btnPanel);
                    border.Child = stack;

                    // adiciona no painel
                    panelModelos.Children.Add(border);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar modelos: " + ex.Message);
            }
        }

        // Evento do botão "Criar" do XAML
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var criarOrcamento = new criar_orca(email, servidor, bd, usr, senha);
            // use ShowDialog para aguardar e recarregar
            bool? r = criarOrcamento.ShowDialog();
            CarregarModelosCriados();
        }

        private void btn_ger_func_Click(object sender, RoutedEventArgs e)
        {
            var gereFunc_Adm = new gereFunc_adm(email, servidor, bd, usr, senha);
            gereFunc_Adm.Show();
        }

        private void btnVoltar_Click(object sender, RoutedEventArgs e)
        {
            foreach (Window janela in Application.Current.Windows.Cast<Window>().ToList())
            {
                if (janela != this)
                    janela.Close();
            }

            var main = new MainWindow();
            main.Show();
            this.Close();
        }

        private void btnPerfil_Click(object sender, RoutedEventArgs e)
        {
            var perfilWin = new PerfilWindow(email, _orcamentoService);
            bool? resultado = perfilWin.ShowDialog();

            if (resultado == true)
            {
                MessageBox.Show("Perfil atualizado com sucesso! Reinicie o sistema para aplicar as mudanças de login (se alterou e-mail).");
                // Se você usa Sessao.email atualize aqui também se preciso
            }
        }
    }
}
