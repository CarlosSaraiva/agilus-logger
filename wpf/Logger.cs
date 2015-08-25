using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Color = System.Windows.Media.Color;

namespace AgilusLogger
{
    public class Logger
    {
        //events
        public event EventHandler OnUpdate;


        //Properties
        private readonly SolidColorBrush _red = new SolidColorBrush(Color.FromArgb(139, 242, 13, 13));

        private readonly ObservableCollection<LoggerService> _loggers;

        public Logger(ListView listView)
        {
            OnUpdate += delegate { };
            ListView = listView;
            _loggers = new ObservableCollection<LoggerService>();
        }

        public Logger()
        {
            OnUpdate += delegate { };
        }

        public int LastSelectedIndex { private get; set; }

        public LoggerService SelectedLogger
        {
            get { return _loggers.ToList().Find(e => e == (LoggerService)ListView.SelectedItem); }
        }

        public ServiceController SelectedService => ((LoggerService)ListView.SelectedItem).Service;

        private ListView ListView { get; set; }

        //Methods
        public void GetServices()
        {
            _loggers.Clear();
            var services = ServiceController.GetServices().ToList();
            var regex = new Regex(@"^agilus(\w*|\d*/)");

            services.ForEach(s =>
            {
                var match = regex.Match(s.ServiceName);
                if (match.Success)
                {
                    _loggers.Add(new LoggerService(s));
                }
            });

            UpdateServicesListViewItem();
        }

        void DelayedAction()
        {
            foreach (LoggerService item in ListView.Items)
            {
                if (item.Status != "Stopped") continue;
                //var container = (ListViewItem)ListView.ItemContainerGenerator.ContainerFromItem(item);
                var i = ListView.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
                if (i != null && !i.Background.Equals(_red))
                    i.Background = _red;
                    OnUpdate?.Invoke(this, new EventArgs());
            }
        }

        private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (ListView.ItemContainerGenerator.Status != System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated) return;
            ListView.ItemContainerGenerator.StatusChanged -= ItemContainerGenerator_StatusChanged;
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Input, new Action(DelayedAction));
        }

        private void UpdateServicesListViewItem()
        {
            ListView.ItemsSource = _loggers;
            ListView.SelectedIndex = LastSelectedIndex;
            ListView.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
            
        }

    }
}