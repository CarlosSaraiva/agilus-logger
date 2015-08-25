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
        //Properties
        private readonly ObservableCollection<LoggerService> Loggers;
        public ServiceController SelectedService => ((LoggerService)ListView.SelectedItem).Service;
        private ListView ListView { get; set; }
        public int LastSelectedIndex { private get; set; }
        private readonly SolidColorBrush _red = new SolidColorBrush(Color.FromArgb(139, 242, 13, 13));

        public LoggerService SelectedLogger
        {
            get { return Loggers.ToList().Find(e => e == (LoggerService) ListView.SelectedItem); }
        }

        public Logger(ListView listView)
        {
            ListView = listView;
            Loggers = new ObservableCollection<LoggerService>();
        }

        //Methods
        public void GetServices()
        {
            Loggers.Clear();
            var services = ServiceController.GetServices().ToList();
            var regex = new Regex(@"^agilus(\w*|\d*/)");

            services.ForEach(s =>
            {
                var match = regex.Match(s.ServiceName);
                if (match.Success)
                {
                    Loggers.Add(new LoggerService(s));
                }
            });

            UpdateServicesListViewItem();
        }

        private void UpdateServicesListViewItem()
        {
            ListView.ItemsSource = Loggers;
            ListView.SelectedIndex = LastSelectedIndex;
            ListView.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
        }

        private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (ListView.ItemContainerGenerator.Status !=                System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated) return;
            ListView.ItemContainerGenerator.StatusChanged -= ItemContainerGenerator_StatusChanged;
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Input, new Action(DelayedAction));
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
            }
        }
    }
}