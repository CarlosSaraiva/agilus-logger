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
    
    public partial class MainWindow : Window
    {
        public static Logger Logger;
        public static string LoggerPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "agilus-logger");
        public static readonly DispatcherTimer UpdateLoggerServicesTimer = new DispatcherTimer();
        public static readonly DispatcherTimer DescricaoTimer = new DispatcherTimer();
        
        public MainWindow()
        {
            InitializeComponent();
            Logger = new Logger(listView);
            UpdateLoggerServicesTimer.Tick += (o, s) =>
            {
                Logger.LastSelectedIndex = listView.SelectedIndex;
                Logger.GetServices();                
            } ;
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

        private void UpdateText()
        {
            if (Logger.SelectedLogger == null) return;
            labelName.Content = Logger.SelectedLogger.Name;
            labelStatus.Content = Logger.SelectedLogger.Status;
        }

        private void Install()
        {
            LoggerPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "agilus-logger");
            string servicePath = Path.Combine(LoggerPath, serviceName.Text);

            if (!Directory.Exists(LoggerPath))
            {
                DirectoryInfo di = Directory.CreateDirectory(LoggerPath);
            }

            if (!Directory.Exists(servicePath))
            {
                DirectoryInfo di = Directory.CreateDirectory(servicePath);
                //Directory.CreateDirectory(Path.Combine(servicePath, "daemon"));
            }

            string[] loggerFiles = Directory.GetFiles("dist\\logger");
            string[] serviceFiles = Directory.GetFiles("dist\\service");
            //File.Copy("C:\\Users\\Carlos\\Documents\\Repositories\\agilus-logger\\node\\package.json", Path.Combine(LoggerPath, "package.json"), true);

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
            string stdout = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
        }

        private void UpdateNpm()
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

        private void Uninstall()
        {
            var service = Logger.SelectedService;

            var process = new Process()
            {
                StartInfo =
                {
                    FileName = "sc.exe ",
                    Arguments = "delete " + service.DisplayName + ".exe"
                }
            };

            if (service.CanStop)
            {
                service.Stop();    
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