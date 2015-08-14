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
        public MainWindow()
        {
            InitializeComponent();            
            var process = Process.GetProcesses();
            var services = ServiceController.GetServices();
            listView.Items.Add(GetServicesListView(services.ToList()));
            listView.MouseDoubleClick += (o, s) => descricao.Text = GetService(services.ToList()).Status.ToString();
            Stop.Click += (o, s) => GetService(services.ToList()).Stop();
            Restart.Click += (o, s) => GetService(services.ToList()).Start();
            NewButton.Click += (o, s) => Install();
        }

        //private ListViewItem GetProcessListView(List<Process> processes)
        //{
        //    var list = new ListViewItem();
        //    processes.ForEach(p => listView.Items.Add(list.Content = p.ProcessName));
        //    return list;
        //}

        //private string GetProcessDetails(List<Process> process)
        //{
        //    var p = process.Find(e => e.ProcessName == (string) listView.SelectedItem);
        //    string details = "Nome:\t" + p.ProcessName + "\n" +
        //                     "PID:\t" + p.Id + "\n";
        //    return details;
        //}

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
            var s = service.Find(e => e.ServiceName == (string)listView.SelectedItem);
            return s;
        }

        private void Install()
        {
            var rootPath = AppDomain.CurrentDomain.BaseDirectory;
            var nodePath = Path.Combine(Environment.GetEnvironmentVariable("ProgramFiles"), "nodejs");
            var loggerPath = Path.GetFullPath("C:\\Users\\Suporte\\Documents\\repositories\\agilus-logger\\node\\src\\install.js");
            var cmd = nodePath + "\\node.exe " + loggerPath + " " + "-p 1430 -n teste";
            var process = new Process
            {
                StartInfo =
                {
                    FileName = Path.Combine(nodePath, "node.exe"),
                    Arguments = Path.Combine(loggerPath),
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                }
            };
            //process.StartInfo.Arguments = "-p 1340 -n teste";
            process.Start();
            string stdout = process.StandardOutput.ReadToEnd();
            MessageBox.Show(process.StandardOutput.ReadToEnd());
            process.WaitForExit();
        }
    }
}
