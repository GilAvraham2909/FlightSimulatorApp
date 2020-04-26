using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using FlightSimulatorApp.Model;

namespace FlightSimulatorApp.ViewModel
{
    public class FlightViewModel : INotifyPropertyChanged
    {
        FlightModel model;

        public event PropertyChangedEventHandler PropertyChanged;

        public FlightViewModel(FlightModel Model)
        {
            this.model = Model;
            this.model.PropertyChanged +=
                delegate (Object sender, PropertyChangedEventArgs e) {
                    NotifyPropertyChanged("VM_" + e.PropertyName);
                };
        }

        public string VM_AirSpeed
        {
            get { return model.AirSpeed; }
        }
        public string VM_GroundSpeed
        {
            get { return model.GroundSpeed; }
        }
        public string VM_VerticalSpeed
        {
            get { return model.VerticalSpeed; }
        }
        public string VM_Roll
        {
            get { return model.Roll; }
        }
        public string VM_Pitch
        {
            get { return model.Pitch; }
        }
        public string VM_Heading
        {
            get { return model.Heading; }
        }
        public string VM_Altitude
        {
            get { return model.Altitude; }
        }
        public string VM_Altimeter
        {
            get { return model.Altimeter; }
        }

        // Map
        public string VM_Latitude
        {
            get { return model.Latitude; }
        }
        public string VM_Longitude
        {
            get { return model.Longitude; }
        }
        public string VM_Error
        {
            get { return model.Error; }
        }

        public string VM_Location
        {
            get
            {
                return model.Location;
            }
        }

        // Commands
        double rudder;
        public double VM_Rudder
        {
            set
            {
                rudder = value;
                model.setRudder(rudder);
            }
        }
        double elevator;
        public double VM_Elevator
        {
            set
            {
                elevator = value;
                model.setElevator(elevator);
            }
        }
        double aileron;
        public double VM_Aileron
        {
            get { return aileron; }
            set
            {
                aileron = value;
                model.setAileron(aileron);
            }
        }
        double throttle;
        public double VM_Throttle
        {
            get { return throttle; }
            set
            {
                throttle = value;
                model.setThrottle(throttle);
            }
        }

        public void NotifyPropertyChanged(String propName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        public void Connect(string ip, int port)
        {
            model.Connect(ip, port);
        }
        public void Disconnect()
        {
            model.Disconnect();
        }
    }
}
