using System;
using System.IO;
using System.Net.Mime;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;

namespace AgilusLogger
{
    using System.Net;
    using System.Threading.Tasks;
    using static Dispatcher;
    using static Node;
    using static Path;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static ServiceManager _manager;

        public static readonly string LoggerPath = Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "agilus-logger");

        public MainWindow()
        {
            InitializeComponent();
            _manager = new ServiceManager(1000000);
            _manager.OnServiceListUpdated += (o, s) => ListView.ItemsSource = _manager.Loggers;
            InstallButton.Click += InstallButton_Click;
            UninstallButton.Click += UninstallButton_Click;
            ListView.ItemContainerGenerator.StatusChanged += ItemContainerGeneratorOnStatusChanged;
            ListView.ItemsSource = _manager.Loggers;
            CancelButton.Click += (o, s) => Tab.SelectedItem = InfoTab;
            NewButton.Click += (o, s) => Tab.SelectedItem = ConfigTab;
            OnExit += (o, s) => CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => TextBlock.Text = s.Message));

            StopButton.Click += (o, s) =>
            {
                var selected = ListView?.Items.GetItemAt(ListView.SelectedIndex) as LoggerService;

                if (selected != null && selected.CanStop)
                {
                    selected.Stop();
                }
            };

            RestartButton.Click += (o, s) =>
            {
                var selected = ListView?.Items.GetItemAt(ListView.SelectedIndex) as LoggerService;

                if (selected.CanPauseAndContinue && selected != null)
                {
                    selected.Pause();
                    selected.Continue();
                }

                if (!selected.CanStop)
                {
                    try
                    {
                        selected.Start();
                    }
                    catch (InvalidOperationException exception)
                    {
                        TextBlock.Text = exception.Message;
                    }
                }
                else
                {
                    RestartService(selected);
                    TextBlock.Text = "Parando Serviço";
                }
            };
        }

        private async void RestartService(LoggerService selected)
        {
            selected.Stop();
            await Task.Run(() => selected.WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Stopped));
            selected.Start();
            TextBlock.Text = "Serviço Reiniciado";
        }

        private void ItemContainerGeneratorOnStatusChanged(object sender, EventArgs eventArgs)
        {
            if (ListView.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
                {
                    foreach (var item in _manager.GetStoppedServices)
                    {
                        var listItem = ListView.ItemContainerGenerator.ContainerFromItem(item) as ListViewItem;
                        if (listItem != null) listItem.Background = new SolidColorBrush(Color.FromArgb(124, 244, 59, 59));
                    }
                }));
            }
        }

        private void UninstallButton_Click(object sender, RoutedEventArgs e)
        {
            SetupNode(NodeAction.Uninstall, (ListView.Items.CurrentItem as LoggerService).EntityName, (ListView.Items.CurrentItem as LoggerService).Port.ToString());
        }

        private void InstallButton_Click(object sender, RoutedEventArgs e)
        {
            SetupNode(NodeAction.Install, ServiceNameField.Text, ServicePortField.Text);
            TextBlock.Text = "Instalando";
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void tab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void TextStatus_TextChanged(object sender, TextChangedEventArgs e)
        {
        }
    }
}