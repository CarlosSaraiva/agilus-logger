using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;


namespace AgilusLogger
{
<<<<<<< HEAD
    using static Path;
=======
    using static Path;    
>>>>>>> b586ab3399de4ef659956f2496229a632fbad652
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
<<<<<<< HEAD
        private static ServiceManager _manager;
=======
        private static ServiceManager Manager;
>>>>>>> b586ab3399de4ef659956f2496229a632fbad652
        public static readonly string LoggerPath = Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "agilus-logger");

        public MainWindow()
        {
            InitializeComponent();
<<<<<<< HEAD
            _manager = new ServiceManager(1000000);
            _manager.OnServiceListUpdated += (o, s) => ListView.ItemsSource = _manager.Loggers;
=======
            Manager = new ServiceManager(1000000);
            Manager.OnServiceListUpdated += (o, s) => ListView.ItemsSource = Manager.Loggers;            
>>>>>>> b586ab3399de4ef659956f2496229a632fbad652
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void tab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TextStatus_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
