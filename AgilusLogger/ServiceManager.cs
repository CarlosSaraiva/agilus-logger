#pragma warning disable CS0472

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
        //Events
        public event EventHandler OnServiceListUpdated;

        public event EventHandler OnTick;

        //Fields-Properties
        private readonly Regex _regex;

        private string Filter { get; }

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

            if (_history.Count > 0 && !actual.SequenceEqual(_history.Last()))
            {
                OnServiceListUpdated?.Invoke(this, new EventArgs());
            }

            _history.Enqueue(actual);

            if (_history.Count > 20)
            {
                _history.Dequeue();
            }

            OnTick?.Invoke(this, new EventArgs());
        }
    }

    public class LogItem : IEquatable<LogItem>
    {
        private string EntityName { get; }
        private int Port { get; }
        private string Status { get; }

        public LogItem(string entityName, int port, string status)
        {
            EntityName = entityName;
            Port = port;
            Status = status;
        }

        public bool Equals(LogItem other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EntityName.Equals(other.EntityName) && Port.Equals(other.Port) && Status.Equals(other.Status);
        }

        public override int GetHashCode()
        {
            var hasEntityName = EntityName?.GetHashCode() ?? 0;
            var hasPort = Port.GetHashCode();
            var hasStatus = Status?.GetHashCode() ?? 0;
            return hasStatus ^ hasPort ^ hasEntityName;
        }
    }
}