using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Threading;
using static System.Diagnostics.Contracts.Contract;

namespace AgilusLogger
{
    using static ServiceController;

    public class ServiceManager
    {
        private long Ticks { get; }
        private DispatcherTimer UpdateTimer { get; set; }
        private ObservableCollection<LoggerInstance> Loggers { get; set; }
        public event EventHandler OnServiceListUpdated;

        public ServiceManager(long ticks)
        {
            Ticks = ticks;  
            SetTimer();
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
                
                OnServiceListUpdated?.Invoke(this, new EventArgs());
            };

            UpdateTimer.Start();
        }

        private void GetLoggersServices()
        {
            GetServices().ToList().ForEach(e =>
            {
                if(e.DisplayName == "Agilus")
                { 
                    Loggers.Add(new LoggerInstance(e));
                }

            });

        }

        private void GetServiceByName(string name)
        {
            throw new NotImplementedException();
        }

        public List<string> GetLoggersList()
        {
            Ensures(Result<List<string>>() != null);
            return new List<string>(Loggers.ToList().Select(e => e.ToString()));
        }
    }
}
