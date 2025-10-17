using ORCA.Services;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
            // panelModelos deve existir no XAML (WrapPanel)
            panelModelos.Children.Clear();

            try
            {
                var modelos = _modeloService.ListarModelosPorCriadorId(_usuarioId);
                foreach (var (id, nome) in modelos)
                {
                    var border = new Border
                    {
                        Width = 260,
                        Height = 90,
                        Margin = new Thickness(10),
                        Background = Brushes.White,
                        CornerRadius = new CornerRadius(6),
                        Padding = new Thickness(10)
                    };

                    var stack = new StackPanel();

                    var title = new TextBlock { Text = nome, FontWeight = FontWeights.Bold, FontSize = 14 };
                    stack.Children.Add(title);

                    var btnPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 8, 0, 0) };

                    var btnEditar = new Button { Content = "Editar", Margin = new Thickness(0, 0, 10, 0), Tag = id };
                    btnEditar.Click += (s, e) =>
                    {
                        int modeloId = (int)(s as Button).Tag;
                        // Abre janela de edição. usar ShowDialog para poder recarregar após fechar.
                        var win = new criar_orca(email, servidor, bd, usr, senha, modeloId);
                        bool? ok = win.ShowDialog();
                        // recarrega lista (independente do DialogResult, é seguro recarregar)
                        CarregarModelosCriados();
                    };

                    var btnExcluir = new Button { Content = "Excluir", Tag = id };
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

                    btnPanel.Children.Add(btnEditar);
                    btnPanel.Children.Add(btnExcluir);

                    stack.Children.Add(btnPanel);
                    border.Child = stack;
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
