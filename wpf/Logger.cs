using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Security.AccessControl;
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
        public readonly ObservableCollection<LoggerService> loggers;

        public ServiceController SelectedService
        {
            get { return ((LoggerService) ListView.SelectedItem).Service; }
        }

        private ListView ListView { get; set; }
        public int LastSelectedIndex { private get; set; }

        public LoggerService SelectedLogger
        {
            get { return loggers.ToList().Find(e => e == (LoggerService) ListView.SelectedItem); }
        }

        public Logger(ListView listView)
        {
            ListView = listView;
            loggers = new ObservableCollection<LoggerService>();
        }

        public void GetServices()
        {
            loggers.Clear();
            var services = ServiceController.GetServices().ToList();
            Regex regex = new Regex(@"^agilus(\w*|\d*/)");

            services.ForEach(s =>
            {
                Match match = regex.Match(s.ServiceName);
                if (match.Success)
                {
                    loggers.Add(new LoggerService(s));
                }
            });

            UpdateServicesListViewItem();
        }

        private void UpdateServicesListViewItem()
        {
            ListView.ItemsSource = loggers;
            ListView.SelectedIndex = LastSelectedIndex;
            ListView.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
        }

        public void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (ListView.ItemContainerGenerator.Status == System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
            {
                ListView.ItemContainerGenerator.StatusChanged -= ItemContainerGenerator_StatusChanged;
                Dispatcher.CurrentDispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input, new Action(DelayedAction));
            }
        }
        
        void DelayedAction()
        {
            foreach (LoggerService item in ListView.Items)
            {
                if (item.Status == "Stopped")
                {
                    var container = (ListViewItem)ListView.ItemContainerGenerator.ContainerFromItem(item);
                    var i = ListView.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
                    if (i != null) i.Background = new SolidColorBrush(Color.FromArgb(139, 77, 27, 27)); 
                }

            }
        }
    }
}