using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Windows.Threading;

namespace AgilusLogger
{
    using static ServiceController;

    public class ServiceManager
    {
        //Fields-Properties
        private string _filter;

        private Regex _regex;

        public event EventHandler OnServiceListUpdated;

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

        public long Ticks { get; }

        private ObservableCollection<LoggerService> Loggers { get; set; }

        private DispatcherTimer UpdateTimer { get; set; }

        //Constructor
        public ServiceManager(long ticks)
        {
            Ticks = ticks;
            SetTimer();
            Filter = @"Agilus\sLogger\s-\s(\w+|\d+)+\s\(porta:\s\d+\s?\)";
            _regex = new Regex(Filter);
        }

        //Methods
        public List<string> GetLoggersList()
        {
            return new List<string>(Loggers.ToList().Select(e => e.ToString()));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CC0022:Should dispose object", Justification = "<Pending>")]
        private ObservableCollection<LoggerService> GetFilteredServices()
        {
            var result = (from service in GetServices().ToList()
                          where _regex.IsMatch(service.DisplayName)
                          select new LoggerService(service));

            return new ObservableCollection<LoggerService>(result);
        }

        private void GetServiceByName(string name)
        {
            throw new NotImplementedException();
        }

        private void SetTimer()
        {
            UpdateTimer = new DispatcherTimer
            {
                Interval = new TimeSpan(Ticks)
            };

            UpdateTimer.Tick += (o, s) =>
            {
                Loggers?.Clear();
                Loggers = GetFilteredServices();
                OnServiceListUpdated?.Invoke(this, new EventArgs());
            };

            UpdateTimer.Start();
        }
    }
}
