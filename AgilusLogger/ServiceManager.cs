using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Threading;

namespace AgilusLogger
{
    using static ServiceController;

    public class ServiceManager
    {
        //Fields-Properties
        private string _filter;

        private readonly Regex _regex;

        public event EventHandler OnServiceListUpdated;

        public event EventHandler OnTick;

        private string Filter
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

        private long Ticks { get; }

        public ObservableCollection<LoggerService> Loggers { get; private set; }

        private List<LoggerService> PreviousLoggersList { get; set; }

        private DispatcherTimer UpdateTimer { get; set; }

        private ListView _list;

        private readonly Queue<IEnumerable<LogItem>> _history;

        //Constructor
        public ServiceManager(long ticks, ListView listView)
        {
            _list = listView;
            Ticks = ticks;            
            //PreviousLoggersList = new List<LoggerService>();
            Filter = @"Agilus\sLogger\s-\s(\w+|\d+)+\s\(porta:\s\d+\s?\)";
            _regex = new Regex(Filter);
            SetTimer();
            
            Loggers = GetFilteredServices();
            PreviousLoggersList = new List<LoggerService>();
            _list.ItemsSource = Loggers;
            _history = new Queue<IEnumerable<LogItem>>();
            //BindingOperations.EnableCollectionSynchronization(Loggers, listView.ItemsSource);
        }

        //Methods
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CC0022:Should dispose object", Justification = "<Pending>")]
        private ObservableCollection<LoggerService> GetFilteredServices()
        {
            var result = (from service in GetServices().ToList()
                          where _regex.IsMatch(service.DisplayName)
                          select new LoggerService(service));
            return new ObservableCollection<LoggerService>(result);
        }

        private void SetTimer()
        {
            UpdateTimer = new DispatcherTimer
            {
                Interval = new TimeSpan(Ticks)
            };

            UpdateTimer.Tick += OnUpdateTimerOnTick;
            UpdateTimer.Start();
        }

        private void OnUpdateTimerOnTick(object o, EventArgs s)
        {
            Loggers = GetFilteredServices();
            bool isEqual;
            var actual = (from l in Loggers select new LogItem(l.Status.ToString(), l.Port, l.EntityName)).ToArray();            

            if (_history.Count > 1 && !actual.SequenceEqual(_history.Peek()))
            {
                OnTick?.Invoke(this, new EventArgs());
            }

            _history.Enqueue(actual);
            if (_history.Count >= 20) _history.Dequeue();

        }
    }

    public class LogItem:IEquatable<LogItem>
    {
        public string EntityName { get; set; }       
        public int Port { get; set; }
        public string Status { get; set; }

        public LogItem(string entityName, int port, string status)
        {
            EntityName = entityName;
            Port = port;
            Status = status;
        }

        public bool Equals(LogItem other)
        {
            if (Object.ReferenceEquals(other, null)) return false;
            if (Object.ReferenceEquals(this, other)) return true;
            return EntityName.Equals(other.EntityName) && Status.Equals(other.Status);
        }

        public override int GetHashCode()
        {
            int hasEntityName = EntityName?.GetHashCode() ?? 0;
            int hasPort = Port == null ? 0 : Port.GetHashCode();
            int hasStatus = Status == null ? 0 : Status.GetHashCode();

            return hasStatus ^ hasPort ^ hasEntityName;
        }
    }

}


