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

    class ServiceManager
    {
        public long Ticks { get; }
        private DispatcherTimer UpdateTimer { get; set; }
        private ObservableCollection<ServiceController> Loggers { get; set; }
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
                GetServices().ToList().ForEach(e => Loggers.Add(e));
                OnServiceListUpdated?.Invoke(this, new EventArgs());
            };

            UpdateTimer.Start();
        }

        private void GetServiceByName(string name)
        {
            throw new NotImplementedException();
        }

    }
}
