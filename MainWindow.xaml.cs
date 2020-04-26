using FlightSimulatorApp.Model;
using FlightSimulatorApp.ViewModel;
using FlightSimulatorApp.View;
using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Shapes;

namespace FlightSimulatorApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public FlightViewModel vm;
        public MainWindow()
        {
            InitializeComponent();
            vm = new FlightViewModel(new FlightModel());
            this.DataContext = vm;
        }

        private void Joystick_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonConnect_Click(object sender, RoutedEventArgs e)
        {
            Connect win = new Connect(vm);
            win.DataContext = vm;
            win.Show();
        }

        private void ButtonDisconnect_Click(object sender, RoutedEventArgs e)
        {
            vm.Disconnect();
        }
    }
}
