using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace AgilusLogger
{
    using static ServiceController;

    public class ServiceManager
    {
        public long Ticks { get; }
        private DispatcherTimer UpdateTimer { get; set; }
        private ObservableCollection<LoggerInstance> Loggers { get; set; }
        private string _filter;
        public string Filter
        {
            get
            {
                return _filter;
            }

            set
            {
                _filter = value;
            }
        }
        public event EventHandler OnServiceListUpdated;
        private readonly Regex _regex;



        public ServiceManager(long ticks)
        {
            Ticks = ticks;
            SetTimer();
            Filter = @"(Agilus\sLogger\s)(?: -\s\w *\s)(?:\(porta:\s\d *\))";
            _regex = new Regex(Filter);
        }

        private void SetTimer()
        {
            UpdateTimer = new DispatcherTimer
            {
                Interval = new TimeSpan(Ticks)
            };

            UpdateTimer.Tick += (o, s) =>
            {
                Loggers.Clear();
                GetFilteredServices();
                OnServiceListUpdated?.Invoke(this, new EventArgs());
            };

            UpdateTimer.Start();
        }

        private void GetFilteredServices()
        {
            Loggers = (from service in GetServices().ToList()
                       let regex = _regex.Match(service.DisplayName)
                       where regex.Success
                       select service) as ObservableCollection<LoggerInstance>;
        }

        public List<string> GetLoggersList()
        {
            return new List<string>(Loggers.ToList().Select(e => e.ToString()));
        }
    }
}
