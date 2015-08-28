#pragma warning disable CS0472

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
        //Events
        public event EventHandler OnServiceListUpdated;

        public event EventHandler OnTick;

        //Fields-Properties
        private string _filter;

        private readonly Regex _regex;

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

        private DispatcherTimer UpdateTimer { get; set; }

        private readonly Queue<IEnumerable<LogItem>> _history;

        public ObservableCollection<LoggerService> Loggers { get; private set; }

        //Constructor
        public ServiceManager(long ticks)
        {
            Ticks = ticks;            
            Filter = @"Agilus\sLogger\s-\s(\w+|\d+)+\s\(porta:\s\d+\s?\)";
            _regex = new Regex(Filter);
            SetTimer();            
            Loggers = GetFilteredServices();            
            _history = new Queue<IEnumerable<LogItem>>();            
        }

        //Methods
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CC0022:Should dispose object", Justification = "<Pending>")]
        private ObservableCollection<LoggerService> GetFilteredServices()
        {
            var result = from service in GetServices().ToList()
                         where _regex.IsMatch(service.DisplayName)
                         select new LoggerService(service);

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
            
            var actual = (
                from l in Loggers
                select new LogItem(l.EntityName, l.Port, l.Status.ToString())).ToArray();

            if (_history.Count > 0 &&  !actual.SequenceEqual(_history.Last()))
            {
                OnServiceListUpdated?.Invoke(this, new EventArgs());
            }

            _history.Enqueue(actual);
            if (_history.Count > 20) _history.Dequeue();
            OnTick?.Invoke(this, new EventArgs());
        }
    }

    public class LogItem:IEquatable<LogItem>
    {
        private string EntityName { get; set; }
        private int Port { get; set; }
        private string Status { get; set; }

        public LogItem(string entityName, int port, string status)
        {
            EntityName = entityName;
            Port = port;
            Status = status;
        }

        public bool Equals(LogItem other)
        {
<<<<<<< HEAD
            if (object.ReferenceEquals(other, null)) return false;
            if (object.ReferenceEquals(this, other)) return true;
=======
            if (Object.ReferenceEquals(other, null)) return false;
            if (Object.ReferenceEquals(this, other)) return true;
>>>>>>> b586ab3399de4ef659956f2496229a632fbad652
            return EntityName.Equals(other.EntityName) && Port.Equals(other.Port) && Status.Equals(other.Status);
        }

        public override int GetHashCode()
        {
            var hasEntityName = EntityName?.GetHashCode() ?? 0;
            var hasPort = Port == null ? 0: Port.GetHashCode();
            var hasStatus = Status == null ? 0 : Status.GetHashCode();
            return hasStatus ^ hasPort ^ hasEntityName;
        }
    }
}