using System;
using System.ComponentModel;
using System.IO;
using System.Net.Mime;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;

namespace AgilusLogger
{
    using System.Diagnostics;
    
    using System.Reflection;
    using System.Security.Principal;
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
            IsAdministrator();
            OnBeginProcess += (o, s) => CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => TextBlock.Text = s.Message));
            _manager = new ServiceManager(1000000);
            _manager.OnServiceListUpdated += (o, s) => ListView.ItemsSource = _manager.Loggers;
            InstallButton.Click += InstallButton_Click;
            UninstallButton.Click += UninstallButton_Click;
            ListView.ItemContainerGenerator.StatusChanged += ItemContainerGeneratorOnStatusChanged;
            ListView.ItemsSource = _manager.Loggers;

            OnBeginProcess += (o, s) => CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => TextBlock.Text = "Inicio da instalação..."));
            OnNpm += (o, s) => CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => TextBlock.Text = "NPM esta atualizando módulos!"));
            OnExit += (o, s) => CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => TextBlock.Text = s.Message));
            OnFailure += (o, s) => CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => TextBlock.Text = s.Message));

            StopButton.Click += (o, s) =>
            {
                var selected = ListView?.Items.GetItemAt(ListView.SelectedIndex) as LoggerService;

                try
                {
                    if (selected.CanStop || selected != null)
                    {
                        selected.Stop();
                    }
                }
                catch (Win32Exception win32Exception)
                {
                    TextBlock.Text = "Problemas ao interromper o serviço: " + win32Exception.Message;
                }
            };

            RestartButton.Click += (o, s) =>
            {
                var selected = ListView?.Items.GetItemAt(ListView.SelectedIndex) as LoggerService;

                if (selected.CanPauseAndContinue)
                {
                    selected.Pause();
                    selected.Continue();
                }else if (!selected.CanStop)
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
            SetupNode(NodeAction.Uninstall, (ListView.Items.CurrentItem as LoggerService).EntityName, (ListView.Items.CurrentItem as LoggerService).Port.ToString());
        }

        private void InstallButton_Click(object sender, RoutedEventArgs e)
        {
            SetupNode(NodeAction.Install, ServiceNameField.Text, ServicePortField.Text);            
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
    }
}