using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Windows.Annotations;
using System.Windows.Controls;
using System.Windows.Data;
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

        //Constructor
        public ServiceManager(long ticks, ListView listView)
        {
            _list = listView;
            Ticks = ticks;            
            PreviousLoggersList = new List<LoggerService>();
            Filter = @"Agilus\sLogger\s-\s(\w+|\d+)+\s\(porta:\s\d+\s?\)";
            _regex = new Regex(Filter);
            SetTimer();
            
            Loggers = GetFilteredServices();            
            _list.ItemsSource = Loggers;
            //BindingOperations.EnableCollectionSynchronization(Loggers, listView.ItemsSource);
        }

        //Methods
        //public IEnumerable<string> GetLoggersList() => new List<string>(Loggers.ToList().Select(e => e.ToString()));

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CC0022:Should dispose object", Justification = "<Pending>")]
        private ObservableCollection<LoggerService> GetFilteredServices()
        {
            var result = (from service in GetServices().ToList()
                          where _regex.IsMatch(service.DisplayName)
                          select new LoggerService(service));
            return new ObservableCollection<LoggerService>(result);
        }

        //private void GetServiceByName(string name)
        //{
        //    throw new NotImplementedException();
        //}

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
            PreviousLoggersList = Loggers?.ToList();
            var temp = GetFilteredServices();            
            bool isUpdated = true;            
            OnTick?.Invoke(this, new EventArgs());

            foreach (var logger in temp.ToList())
            {
                if(PreviousLoggersList != null)
                {
                    foreach (var prevLogger in PreviousLoggersList)
                    {
                        if (prevLogger.Status == logger.Status && prevLogger.EntityName == logger.EntityName)
                        {
                            isUpdated = true;
                        }
                        else
                        {
                            isUpdated = false;
                        }
                    }
                }
              
            }

            if (!isUpdated)
            {
                //Loggers?.Clear();
                Loggers = temp;
                //Loggers.CollectionChanged += NewShit;

               _list.ItemsSource = Loggers;
            }

        }

        //private void NewShit(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    foreach(var items in _list.Items) 
        //}
    }
}
