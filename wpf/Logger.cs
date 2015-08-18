using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace AgilusLogger
{
    public class Logger
    {
        private ObservableCollection<LoggerService> loggers;
        private LoggerService selectedLogger;
        public ServiceController SelectedService
        {
            get { return ((LoggerService) ListView.SelectedItem).Service; }
        }
        public ListView ListView { get; private set; }
        public int LastSelectedIndex { get; set; }
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

        public void UpdateServicesListViewItem()
        {            
            ListView.ItemsSource = loggers;
            ListView.SelectedIndex = LastSelectedIndex;
        }

    }
}