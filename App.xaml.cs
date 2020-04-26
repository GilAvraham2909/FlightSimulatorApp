﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FlightSimulatorApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //public FlightModel flightModel;
        private void Application_Startup(Object sender, StartupEventArgs e)
        {
            //flightModel = new FlightModel();

            Window mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}