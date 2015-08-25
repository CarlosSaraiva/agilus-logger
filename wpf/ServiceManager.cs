using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Threading;
<<<<<<< HEAD
using static System.Diagnostics.Contracts.Contract;
=======
>>>>>>> 4d94fbc99207fb19bd2054c5bbe0539c63e8d5a9

namespace AgilusLogger
{
    using static ServiceController;

<<<<<<< HEAD
    public class ServiceManager
    {
        private long Ticks { get; }
        private DispatcherTimer UpdateTimer { get; set; }
        private ObservableCollection<LoggerInstance> Loggers { get; set; }
=======
    class ServiceManager
    {
        public long Ticks { get; }
        private DispatcherTimer UpdateTimer { get; set; }
        private ObservableCollection<ServiceController> Loggers { get; set; }
>>>>>>> 4d94fbc99207fb19bd2054c5bbe0539c63e8d5a9
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
<<<<<<< HEAD
                
=======
                GetServices().ToList().ForEach(e => Loggers.Add(e));
>>>>>>> 4d94fbc99207fb19bd2054c5bbe0539c63e8d5a9
                OnServiceListUpdated?.Invoke(this, new EventArgs());
            };

            UpdateTimer.Start();
        }

<<<<<<< HEAD
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

=======
>>>>>>> 4d94fbc99207fb19bd2054c5bbe0539c63e8d5a9
        private void GetServiceByName(string name)
        {
            throw new NotImplementedException();
        }

<<<<<<< HEAD
        public List<string> GetLoggersList()
        {
            Ensures(Result<List<string>>() != null);
            return new List<string>(Loggers.ToList().Select(e => e.ToString()));
        }
=======
>>>>>>> 4d94fbc99207fb19bd2054c5bbe0539c63e8d5a9
    }
}
