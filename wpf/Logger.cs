using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace AgilusLogger
{
    public class Logger
    {
        private readonly ObservableCollection<LoggerService> loggers;
        public ServiceController SelectedService
        {
            get { return ((LoggerService) ListView.SelectedItem).Service; }
        }
        private ListView ListView { get; set; }
        public int LastSelectedIndex { private get; set; }
        public LoggerService SelectedLogger
        {
            get
            {
                return  loggers.ToList().Find(e => e == (LoggerService)ListView.SelectedItem);
            }
        }
        public Logger(ListView listView)
        {
            ListView = listView;                     
            loggers = new ObservableCollection<LoggerService>();
        }
        public void GetServices()
        {
            loggers.Clear();
            var services = ServiceController.GetServices().ToList();
            Regex regex = new Regex(@"^agilus(\w*|\d*/)");

            services.ForEach(s =>
            {
                Match match = regex.Match(s.ServiceName);
                if (match.Success)
                {
                    loggers.Add(new LoggerService(s));
                }
            });

            UpdateServicesListViewItem();
        }
        private void UpdateServicesListViewItem()
        {            
            ListView.ItemsSource = loggers;
            ListView.SelectedIndex = LastSelectedIndex;
        }
    }
}