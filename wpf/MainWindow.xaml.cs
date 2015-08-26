using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
        //private static Logger _logger;
        //private static readonly string LoggerPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "agilus-logger");
        //private static readonly DispatcherTimer UpdateLoggerServicesTimer = new DispatcherTimer();
        //private static readonly DispatcherTimer DescricaoTimer = new DispatcherTimer();
        //private static TaskbarIcon _tray;

        private readonly ServiceManager _serviceManager = new ServiceManager(1000);
        
        public MainWindow()
        {
            InitializeComponent();
            TextStatus.Text = _serviceManager.GetLoggersList().FirstOrDefault();
            //_logger = new Logger(ListView);
            //InitializeEvents();
            //_tray = (TaskbarIcon)FindResource("NotifyIcon");
            //tray.Icon = new Icon("C:\\Users\\Suporte\\Documents\\repositories\\agilus-logger\\wpf\\Assets\\warning.ico");
            //ElevateMe();
        }

        //private void UpdateText()
        //{
            //if (_logger.SelectedLogger == null) return;
            //LabelName.Content = _logger.SelectedLogger.EntityName;
            //LabelStatus.Content = _logger.SelectedLogger.Status;
            //LabelServiceName.Content = _logger.SelectedLogger.Service.ServiceName;
            //LabelMachineName.Content = _logger.SelectedLogger.Service.MachineName;
            //LabelMachineLifeTime.Content = _logger.SelectedLogger.Lifetime;
        //}

        //private void InitializeEvents()
        //{
        //    //UpdateLoggerServicesTimer.Tick += (o, s) =>
        //    //{
        //    //    _logger.LastSelectedIndex = ListView.SelectedIndex;
        //    //    _logger.GetServices();
        //    //};
        //    //UpdateLoggerServicesTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);
        //    //UpdateLoggerServicesTimer.Start();
        //    //StopButton.Click += (o, s) => _logger.SelectedService.Stop();
        //    //ListView.MouseDoubleClick += (o, s) => DescricaoTimer.Start();
        //    //RestartButton.Click += (o, s) => _logger.SelectedService.Refresh();
        //    //NewButton.Click += (o, s) => Tab.SelectedItem = ConfigTab;
        //    //InstallButton.Click += (o, s) => Install();
        //    //CancelButton.Click += (o, s) => Tab.SelectedItem = InfoTab;
        //    //UninstallButton.Click += (o, s) => Uninstall();
        //    //DescricaoTimer.Tick += (o, s) => UpdateText();
        //    //DescricaoTimer.Interval = new TimeSpan(0, 0, 1);
        //    //DescricaoTimer.Start();
        //    //Application.Current.Startup += (o, s) => ElevateMe();
        //    //_logger.OnUpdate += (o, s) => Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => MessageBox.Show(@"Serviço Parado")));
        //}

        //private void Install()
        //{            
        //    var servicePath = Path.Combine(LoggerPath, ServiceName.Text);

        //    if (!Directory.Exists(LoggerPath))
        //    {
        //        Directory.CreateDirectory(LoggerPath);
        //    }

        //    if (!Directory.Exists(servicePath))
        //    {
        //        Directory.CreateDirectory(servicePath);
        //        //Directory.CreateDirectory(Path.Combine(servicePath, "daemon"));
        //    }

        //    var loggerFiles = Directory.GetFiles("dist\\logger");
        //    var serviceFiles = Directory.GetFiles("dist\\service");

        //    foreach (var s in loggerFiles)
        //    {
        //        if (s == null) continue;
        //        var destFile = Path.Combine(LoggerPath, Path.GetFileName(s));
        //        File.Copy(s, destFile, true);
        //    }

        //    foreach (var s in serviceFiles)
        //    {
        //        if (s == null) continue;
        //        var destFile = Path.Combine(servicePath, Path.GetFileName(s));
        //        File.Copy(s, destFile, true);
        //    }

        //    UpdateNpm();

        //    using (
        //    var process = new Process
        //    {
        //        StartInfo =
        //        {
        //            FileName = Path.Combine(LoggerPath, "agilus.cmd"),
        //            Arguments = $"-p {ServicePort.Text} -n {ServiceName.Text}",
        //            WindowStyle = ProcessWindowStyle.Hidden,
        //            UseShellExecute = false,
        //            CreateNoWindow = true,
        //            RedirectStandardOutput = true
        //        }
        //    })
        //    {
        //        process.Start();
        //        _tray.ShowBalloonTip("NPM", "Instalando NPM", BalloonIcon.Info);
        //        string stdout = process.StandardOutput.ReadToEnd();
        //        TextStatus.Text = stdout;
        //    }
        //    //process.WaitForExit();
        //}

        //private void UpdateNpm()
        //{
        //    using (var process = new Process
        //    {
        //        StartInfo =
        //        {
        //            FileName = "npm.cmd",
        //            Arguments = @"--prefix " + LoggerPath + " install",
        //            WindowStyle = ProcessWindowStyle.Hidden,
        //            UseShellExecute = false,
        //            CreateNoWindow = true,
        //            RedirectStandardOutput = true

        //        }
        //    })
        //    {
        //        process.Start();
        //        //string stdout = process.StandardOutput.ReadToEnd();
        //        TextStatus.Text = "Executando NPM";
        //    }
        //    //process.WaitForExit();
        //}

        //private static void Uninstall()
        //{
        //    var service = _logger.SelectedLogger;
        //    using (var process = new Process()
        //    {
        //        StartInfo =
        //        {
        //            FileName = Path.Combine(LoggerPath, "uninstall.cmd"),
        //            Arguments = $"-n {service.EntityName} -s {service.Service.ServiceName}",
        //            UseShellExecute = true,
        //            CreateNoWindow = true,
        //            RedirectStandardOutput = false
        //        }
        //    })
        //    {
        //        if (service.Service.CanStop)
        //        {
        //            service.Service.Stop();
        //        }

        //        process.Start();
        //    }
        //    // var stdout = process.StandardOutput.ReadToEnd();
        //    //MessageBox.Show(stdout);
        //    //process.WaitForExit();

        //    //Directory.Delete(Path.Combine(LoggerPath, service.EntityName),true);
        //}

        //private static void ElevateMe()
        //{
        //    var info = new ProcessStartInfo(Assembly.GetEntryAssembly().Location,  "  --engage ")
        //    {
        //        Verb = "runas"
        //    };

        //    using (
        //    var process = new Process
        //    {
        //        EnableRaisingEvents = true,
        //        StartInfo = info
        //    })
        //    {
        //        process.Start();
        //        process.WaitForExit();
        //    }
        //}

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