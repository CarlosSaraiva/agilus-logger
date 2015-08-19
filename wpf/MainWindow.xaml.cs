using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace AgilusLogger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow
    {
        public static Logger Logger;
        public static readonly string LoggerPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "agilus-logger");
        public static readonly DispatcherTimer UpdateLoggerServicesTimer = new DispatcherTimer();
        public static readonly DispatcherTimer DescricaoTimer = new DispatcherTimer();
        
        public MainWindow()
        {
            InitializeComponent();
            Logger = new Logger(listView);
            InitializeEvents();

        }
        private void UpdateText()
        {
            if (Logger.SelectedLogger == null) return;
            labelName.Content = Logger.SelectedLogger.EntityName;
            labelStatus.Content = Logger.SelectedLogger.Status;
            labelServiceName.Content = Logger.SelectedLogger.Service.ServiceName;
            labelMachineName.Content = Logger.SelectedLogger.Service.MachineName;
        }
        private void InitializeEvents()
        {
            UpdateLoggerServicesTimer.Tick += (o, s) =>
            {
                Logger.LastSelectedIndex = listView.SelectedIndex;
                Logger.GetServices();
            };
            UpdateLoggerServicesTimer.Interval = new TimeSpan(0, 0, 0, 0, 300);
            UpdateLoggerServicesTimer.Start();
            StopButton.Click += (o, s) => Logger.SelectedService.Stop();
            listView.MouseDoubleClick += (o, s) => DescricaoTimer.Start();
            RestartButton.Click += (o, s) => Logger.SelectedService.Refresh();
            NewButton.Click += (o, s) => tab.SelectedItem = configTab;
            InstallButton.Click += (o, s) => Install();
            CancelButton.Click += (o, s) => tab.SelectedItem = infoTab;
            UninstallButton.Click += (o, s) => Uninstall();
            DescricaoTimer.Tick += (o, s) => UpdateText();
            DescricaoTimer.Interval = new TimeSpan(0, 0, 1);
            DescricaoTimer.Start();
            
        }
        private void Install()
        {            
            string servicePath = Path.Combine(LoggerPath, serviceName.Text);

            if (!Directory.Exists(LoggerPath))
            {
                Directory.CreateDirectory(LoggerPath);
            }

            if (!Directory.Exists(servicePath))
            {
                Directory.CreateDirectory(servicePath);
                //Directory.CreateDirectory(Path.Combine(servicePath, "daemon"));
            }

            string[] loggerFiles = Directory.GetFiles("dist\\logger");
            string[] serviceFiles = Directory.GetFiles("dist\\service");
            
            foreach (string s in loggerFiles)
            {
                if (s == null) continue;
                var destFile = Path.Combine(LoggerPath, Path.GetFileName(s));
                File.Copy(s, destFile, true);
            }

            foreach (string s in serviceFiles)
            {
                if (s == null) continue;
                var destFile = Path.Combine(servicePath, Path.GetFileName(s));
                File.Copy(s, destFile, true);
            }

            UpdateNpm();

            var process = new Process
            {
                StartInfo =
                {
                    FileName = Path.Combine(LoggerPath, "agilus.cmd"),
                    Arguments = @"-p " + servicePort.Text + " -n " + serviceName.Text,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                }
            };
            
            process.Start();
            //string stdout = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
        }
        private static void UpdateNpm()
        {
            var process = new Process
            {
                StartInfo =
                {
                    FileName = "npm.cmd",
                    Arguments = @"--prefix " + LoggerPath + " install",
                    //WindowStyle = ProcessWindowStyle.Hidden,
                    //UseShellExecute = false,
                    //RedirectStandardOutput = true
                }

            };
            process.Start();
            process.WaitForExit();

        }
        private static void Uninstall()
        {
            var service = Logger.SelectedLogger;

            var process = new Process()
            {
                StartInfo =
                {
                    FileName = Path.Combine(LoggerPath, "uninstall.cmd"),
                    Arguments = "-n " + service.EntityName + "-s " + service.Service.ServiceName
                }
            };

            if (service.Service.CanStop)
            {
                service.Service.Stop();    
            }
            

            process.Start();
            process.WaitForExit();

        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void tab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}