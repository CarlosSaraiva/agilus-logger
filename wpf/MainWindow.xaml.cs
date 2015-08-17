using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static ServiceController[] Services;
        public static string LoggerPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "agilus-logger");
        public static DispatcherTimer timer = new DispatcherTimer();
        public static DispatcherTimer descricaoTimer = new DispatcherTimer();        

        public MainWindow()
        {
            InitializeComponent();                                             
            StopButton.Click += (o, s) => GetSelectedService(Services.ToList()).Stop();
            RestartButton.Click += (o, s) =>
            {
                var service = GetSelectedService(Services.ToList());
                if (service.CanStop)
                {
                    service.Stop();
                }
                service.Start();
            };
            NewButton.Click += (o, s) => tab.SelectedItem = configTab;
            InstallButton.Click += (o, s) => Install();
            CancelButton.Click += (o, s) => tab.SelectedItem = infoTab;
            UninstallButton.Click += (o, s) => Uninstall();
            
            timer.Tick += (o, s) => UpdateList();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 300);
            timer.Start();

            descricaoTimer.Tick += UpdateText;
            descricaoTimer.Interval = new TimeSpan(0, 0, 1);            

        }

        private void UpdateText(object sender, EventArgs e)
        {
            UpdateText();
        }

        private void UpdateText()
        {
            if (GetSelectedService(Services.ToList()) != null)
            {
                var service = GetSelectedService(Services.ToList());
                labelName.Content = GetName(service.DisplayName);
                labelStatus.Content = service.Status;
            }
        }

        private string GetName(string name)
        {
            Regex regex = new Regex(@"(Agilus Logger)(?:\s\-\s)(\w*\s)");
            Match match = regex.Match(name);
            return match.Groups[2].Value;
        }

        private ListViewItem GetServicesListView(List<ServiceController> service)
        {
            Regex regex = new Regex(@"^agilus(\w*|\d*/)");
            service.ForEach(s =>
            {
                Match match = regex.Match(s.ServiceName);
                if (match.Success)
                {
                    ListViewItem[] item = new ListViewItem[];          
                    
                    listView.Items.Add(item.Content = GetName(s.DisplayName));
                    
                }
            });

            gridView.
            
            return list;
        }

        private ServiceController GetSelectedService(List<ServiceController> service)
        {
            var s = service.Find(e => e.ServiceName == (string)listView.SelectedItem);
            return s;
        }

        private void UpdateList()
        {
            var selected = listView.SelectedItem;
            Services = ServiceController.GetServices();
            listView.Items.Clear();
            listView.Items.Add(GetServicesListView(Services.ToList()));

            listView.MouseDoubleClick += (o, s) =>
            {
                UpdateText();
                if (!descricaoTimer.IsEnabled)
                {
                    descricaoTimer.Stop();
                }

                descricaoTimer.Start();    
            };
            listView.SelectedItem = selected;
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
            var service = GetSelectedService(Services.ToList());

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