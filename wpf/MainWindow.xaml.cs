using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Hardcodet.Wpf.TaskbarNotification;

namespace AgilusLogger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private static Logger logger;
        private static readonly string LoggerPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "agilus-logger");
        private static readonly DispatcherTimer UpdateLoggerServicesTimer = new DispatcherTimer();
        private static readonly DispatcherTimer DescricaoTimer = new DispatcherTimer();
        private static TaskbarIcon tray;
        
        public MainWindow()
        {
            InitializeComponent();
            logger = new Logger(listView);
            InitializeEvents();
            tray = (TaskbarIcon)FindResource("NotifyIcon");
            tray.Icon = new Icon("C:\\Users\\Suporte\\Documents\\repositories\\agilus-logger\\wpf\\Assets\\warning.ico");
        }
        private void UpdateText()
        {
            if (logger.SelectedLogger == null) return;
            labelName.Content = logger.SelectedLogger.EntityName;
            labelStatus.Content = logger.SelectedLogger.Status;
            labelServiceName.Content = logger.SelectedLogger.Service.ServiceName;
            labelMachineName.Content = logger.SelectedLogger.Service.MachineName;
            labelMachineLifeTime.Content = logger.SelectedLogger.Lifetime;
        }
        private void InitializeEvents()
        {
            UpdateLoggerServicesTimer.Tick += (o, s) =>
            {
                logger.LastSelectedIndex = listView.SelectedIndex;
                logger.GetServices();
            };
            UpdateLoggerServicesTimer.Interval = new TimeSpan(0, 0, 0, 0, 300);
            UpdateLoggerServicesTimer.Start();
            StopButton.Click += (o, s) => logger.SelectedService.Stop();
            listView.MouseDoubleClick += (o, s) => DescricaoTimer.Start();
            RestartButton.Click += (o, s) => logger.SelectedService.Refresh();
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

            var loggerFiles = Directory.GetFiles("dist\\logger");
            var serviceFiles = Directory.GetFiles("dist\\service");
            
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
                    CreateNoWindow = true,
                    RedirectStandardOutput = true
                }
            };
            
            process.Start();
            tray.ShowBalloonTip("NPM", "Instalando NPM",  BalloonIcon.Info);
            string stdout = process.StandardOutput.ReadToEnd();
            TextStatus.Text = stdout;
            //process.WaitForExit();
        }
        private void UpdateNpm()
        {
            var process = new Process
            {
                StartInfo =
                {
                    FileName = "npm.cmd",
                    Arguments = @"--prefix " + LoggerPath + " install",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true

                }
            };
            process.Start();
            string stdout = process.StandardOutput.ReadToEnd();
            TextStatus.Text = "Executando NPM";
            //process.WaitForExit();
        }
        private static void Uninstall()
        {
            var service = logger.SelectedLogger;
            var process = new Process()
            {
                StartInfo =
                {
                    FileName = Path.Combine(LoggerPath, "uninstall.cmd"),
                    Arguments = "-n " + service.EntityName + "-s " + service.Service.ServiceName,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true
                }
            };

            if (service.Service.CanStop)
            {
                service.Service.Stop();    
            }
            
            process.Start();
            var stdout = process.StandardOutput.ReadToEnd();
            MessageBox.Show(stdout);
            //process.WaitForExit();

            //Directory.Delete(Path.Combine(LoggerPath, service.EntityName),true);
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