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
        //public static EventHandler UT;

        public MainWindow()
        {
            InitializeComponent();                                             
            StopButton.Click += (o, s) => GetSelectedService(Services.ToList()).Stop();
            RestartButton.Click += (o, s) => GetSelectedService(Services.ToList()).Start();
            NewButton.Click += (o, s) => tab.SelectedItem = configTab;
            InstallButton.Click += (o, s) => Install();
            CancelButton.Click += (o, s) => tab.SelectedItem = infoTab;
            
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
                descricao.Text = GetSelectedService(Services.ToList()).Status.ToString();
            }
        }


        private ListViewItem GetServicesListView(List<ServiceController> service)
        {
            var list = new ListViewItem();
            Regex regex = new Regex(@"^agilus(\w*|\d*/)");
            
            service.ForEach(s =>
            {
                Match match = regex.Match(s.ServiceName);
                if (match.Success)
                    listView.Items.Add(list.Content = s.ServiceName);
            });
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
            //Dispatcher.BeginInvoke(new Action(() => listView.Items.Clear()));
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

            string[] loggerFiles = Directory.GetFiles("C:\\Users\\Carlos\\Documents\\Repositories\\agilus-logger\\node\\build\\logger");
            string[] serviceFiles = Directory.GetFiles("C:\\Users\\Carlos\\Documents\\Repositories\\agilus-logger\\node\\build\\service");
            File.Copy("C:\\Users\\Carlos\\Documents\\Repositories\\agilus-logger\\node\\package.json", Path.Combine(LoggerPath, "package.json"), true);

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
            
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}