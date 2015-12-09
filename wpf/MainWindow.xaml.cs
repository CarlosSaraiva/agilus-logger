using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using AgilusLogger.Classes;

namespace AgilusLogger
{
    using System.Diagnostics;
    
    using System.Reflection;
    using System.Security.Principal;
    using System.Threading.Tasks;
    using static Dispatcher;
    using static NodeService;
    using static Path;

    public partial class MainWindow : Window
    {
        private static ServiceManager _manager;

        public static readonly string LoggerPath = Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "agilus-logger");

        public MainWindow()
        {
            InitializeComponent();
            IsAdministrator();

            OnBeginProcess += (o, s) => CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => TextBlock.Text = s.Message));
            _manager = new ServiceManager(1000000);
            _manager.OnServiceListUpdated += (o, s) => ListView.ItemsSource = _manager.Loggers;
            InstallButton.Click += InstallButton_Click;
            UninstallButton.Click += UninstallButton_Click;
            ListView.ItemContainerGenerator.StatusChanged += ItemContainerGeneratorOnStatusChanged;
            ListView.ItemsSource = _manager.Loggers;

            OnBeginProcess += (o, s) => CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => TextBlock.Text = s.Message));
            OnUpdate += (o, s) => CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => TextBlock.Text = s.Message));
            OnExit += (o, s) => CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => TextBlock.Text = s.Message));
            OnFailure += (o, s) => CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => TextBlock.Text = s.Message));

            StopButton.Click += (o, s) =>
            {                
                try
                {
                    var selected = ListView?.Items.GetItemAt(ListView.SelectedIndex) as LoggerService;
                    selected?.Stop();
                }
                catch (ArgumentOutOfRangeException argumentOutOfRangeException)
                {                    
                    TextBlock.Text = $"Nenhum item selecionado! ({argumentOutOfRangeException.Message})";
                }
                catch (Win32Exception win32Exception)
                {
                    TextBlock.Text = "Problemas ao interromper o serviço: " + win32Exception.Message;
                }
                catch (InvalidOperationException invalidOperationException)
                {
                    TextBlock.Text = $"Acesso negado: {invalidOperationException.Message}";
                }
            };

            RestartButton.Click += (o, s) =>
            {               
                try
                {
                    var selected = ListView?.Items.GetItemAt(ListView.SelectedIndex) as LoggerService;
                    if (selected != null && selected.CanPauseAndContinue)
                    {
                        selected.Pause();
                        selected.Continue();
                    }
                    else if (selected != null && !selected.CanStop)
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
                        TextBlock.Text = "Reiniciando Serviço";
                    }
                }
                catch (InvalidOperationException invalidOperationException)
                {
                    TextBlock.Text = $"Operação inválida: {invalidOperationException.Message}";
                }
                catch (ArgumentOutOfRangeException argumentOutOfRangeException)
                {
                    TextBlock.Text = $"Nenhum item selecionado! ({argumentOutOfRangeException.Message})";
                }
            };
        }

        private void IsAdministrator()
        {
            var wi = WindowsIdentity.GetCurrent();
            var wp = new WindowsPrincipal(wi);

            bool runAsAdmin = wp.IsInRole(WindowsBuiltInRole.Administrator);

            if (!runAsAdmin)
            {
                // It is not possible to launch a ClickOnce app as administrator directly,
                // so instead we launch the app as administrator in a new process.
                var processInfo = new ProcessStartInfo(Assembly.GetExecutingAssembly().CodeBase);

                // The following properties run the new process as administrator
                processInfo.UseShellExecute = true;
                processInfo.Verb = "runas";

                // Start the new process
                try
                {
                    Process.Start(processInfo);
                }
                catch (Exception)
                {
                    // The user did not allow the application to run as administrator
                    MessageBox.Show("Sorry, but I don't seem to be able to start " +
                       "this program with administrator rights!");
                }

                // Shut down the current process
                Application.Current.Shutdown();
            }            
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
            var loggerService = ListView.SelectedItem  as LoggerService;
            if (loggerService != null)
            {
                UninstallService(loggerService);
            }
            else
            {
                TextBlock.Text = "Nenhum item selecionado!";
            }
        }

        private void InstallButton_Click(object sender, RoutedEventArgs e)
        {
            SetupNode(ServiceNameField.Text, ServicePortField.Text);
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

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void textBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }
    }
}