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

    /// <summary>
    /// 
    /// </summary>
    public class ServiceManager
    {
        /// <summary>
        /// Occurs when [on service list updated].
        /// </summary>
        public event EventHandler OnServiceListUpdated;

        /// <summary>
        /// Occurs when [on tick].
        /// </summary>
        public event EventHandler OnTick;

        /// <summary>
        /// Occurs when [on stopped service].
        /// </summary>
        public event EventHandler OnStoppedService;

        /// <summary>
        /// Occurs when [_on test].
        /// </summary>
        private event EventHandler _onTest;

        /// <summary>
        /// Occurs when [on test].
        /// </summary>
        public event EventHandler OnTest
        {
            add
            {
                
                _onTest += value;
            }
            remove
            {
                _onTest -= value;
            }
        }

        /// <summary>
        /// The _regex
        /// </summary>
        private readonly Regex _regex;

        /// <summary>
        /// Gets the filter.
        /// </summary>
        /// <value>
        /// The filter.
        /// </value>
        private string Filter { get; }

        /// <summary>
        /// Gets the ticks.
        /// </summary>
        /// <value>
        /// The ticks.
        /// </value>
        private long Ticks { get; }

        /// <summary>
        /// Gets or sets the update timer.
        /// </summary>
        /// <value>
        /// The update timer.
        /// </value>
        private DispatcherTimer UpdateTimer { get; set; }

        /// <summary>
        /// The _history
        /// </summary>
        private readonly Queue<IEnumerable<LogItem>> _history;

        /// <summary>
        /// Gets the loggers.
        /// </summary>
        /// <value>
        /// The loggers.
        /// </value>
        public ObservableCollection<LoggerService> Loggers { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceManager"/> class.
        /// </summary>
        /// <param name="ticks">The ticks.</param>
        public ServiceManager(long ticks)
        {
            Ticks = ticks;
            Filter = @"Agilus\sLogger\s-\s(\w+|\d+)+\s\(porta:\s\d+\s?\)";
            _regex = new Regex(Filter);
            SetTimer();
            Loggers = GetFilteredServices();
            _history = new Queue<IEnumerable<LogItem>>();
        }

        /// <summary>
        /// Gets the filtered services.
        /// </summary>
        /// <returns></returns>
        private ObservableCollection<LoggerService> GetFilteredServices()
        {
            var result = from service in GetServices().ToList()
                         where _regex.IsMatch(service.DisplayName)
                         select new LoggerService(service);

            return new ObservableCollection<LoggerService>(result);
        }

        /// <summary>
        /// Sets the timer.
        /// </summary>
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
            _onTest?.Invoke(this, new EventArgs());
        }

        public IEnumerable<LoggerService> GetStoppedServices => Loggers.Where(l => l.Status.ToString() == "Stopped");
    }

    public class LogItem : IEquatable<LogItem>
    {
        /// <summary>
        /// Gets the name of the entity.
        /// </summary>
        /// <value>
        /// The name of the entity.
        /// </value>
        private string EntityName { get; }

        /// <summary>
        /// Gets the port.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        private int Port { get; }

        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        private string Status { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogItem" /> class.
        /// </summary>
        /// <param name="entityName">Name of the entity.</param>
        /// <param name="port">The port.</param>
        /// <param name="status">The status.</param>
        public LogItem(string entityName, int port, string status)
        {
            EntityName = entityName;
            Port = port;
            Status = status;
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public bool Equals(LogItem other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EntityName.Equals(other.EntityName) && Port.Equals(other.Port) && Status.Equals(other.Status);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            var hasEntityName = EntityName?.GetHashCode() ?? 0;
            var hasPort = Port.GetHashCode();
            var hasStatus = Status?.GetHashCode() ?? 0;
            return hasStatus ^ hasPort ^ hasEntityName;
        }
    }
}