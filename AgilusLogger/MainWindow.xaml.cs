using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace AgilusLogger
{
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

            ListView.ItemsSource = _manager.Loggers;

            var teste = ListView.Items;
        }

        private void UninstallButton_Click(object sender, RoutedEventArgs e)
        {
            SetupNode(NodeAction.Uninstall, (ListView.Items.CurrentItem as LoggerService).EntityName, (ListView.Items.CurrentItem as LoggerService).Port.ToString());
        }

        private void InstallButton_Click(object sender, RoutedEventArgs e)
        {
            SetupNode(NodeAction.Install, ServiceNameField.Text, ServicePortField.Text);
            TextStatus.Text = "Instalando";
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