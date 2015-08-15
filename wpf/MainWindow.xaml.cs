using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static ServiceController[] Services;
        public MainWindow()
        {
            InitializeComponent();                                             
            StopButton.Click += (o, s) => GetService(Services.ToList()).Stop();
            RestartButton.Click += (o, s) => GetService(Services.ToList()).Start();
            NewButton.Click += (o, s) => tab.SelectedItem = configTab;
            InstallButton.Click += (o, s) => Install();
            CancelButton.Click += (o, s) => tab.SelectedItem = infoTab;
            UpdateList();
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

        private ServiceController GetService(List<ServiceController> service)
        {
            MessageBox.Show((string) listView.SelectedItem);
            var s = service.Find(e => e.ServiceName == (string)listView.SelectedItem);
            return s;
        }

        private void UpdateList()
        {
            Services = ServiceController.GetServices();
            listView.Items.Clear();
            listView.Items.Add(GetServicesListView(Services.ToList()));
            listView.MouseDoubleClick += (o, s) => descricao.Text = GetService(Services.ToList()).Status.ToString();
            
        }

        private void Install()
        {
            string loggerPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "agilus-logger");
            string servicePath = Path.Combine(loggerPath, serviceName.Text);

            if (!Directory.Exists(loggerPath))
            {
                DirectoryInfo di = Directory.CreateDirectory(loggerPath);
            }

            if (!Directory.Exists(servicePath))
            {
                DirectoryInfo di = Directory.CreateDirectory(servicePath);
            }

 
            string[] files = Directory.GetFiles("C:\\Users\\Carlos\\Documents\\Repositories\\agilus-logger\\node\\build");
            File.Copy("C:\\Users\\Carlos\\Documents\\Repositories\\agilus-logger\\node\\package.json", Path.Combine(loggerPath, "package.json"));

            foreach (string s in files)
            {
                if (s != null)
                {
                    var destFile = Path.Combine(servicePath, Path.GetFileName(s));
                    File.Copy(s, destFile, true);
                }
            }


            var process = new Process
            {
                StartInfo =
                {
                    FileName = "agilus.cmd",
                    Arguments = @"-p " + servicePort.Text + " -n " + serviceName.Text,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                }
            };
            
            process.Start();
            string stdout = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            UpdateList();
        }

        private void Uninstall()
        {
            
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
