using System;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.ComponentModel;

namespace FlightSimulatorApp.Model
{
    public class FlightModel : INotifyPropertyChanged
    {
        bool isConnect = false;
        // Sensors
        volatile string airspeed;
        public string AirSpeed
        {
            get { return airspeed; }
            set
            {
                airspeed = value;
                NotifyPropertyChanged("AirSpeed");
            }
        }
        volatile string groundSpeed;
        public string GroundSpeed
        {
            get { return groundSpeed; }
            set
            {
                groundSpeed = value;
                NotifyPropertyChanged("GroundSpeed");
            }
        }
        volatile string verticalSpeed;
        public string VerticalSpeed
        {
            get { return verticalSpeed; }
            set
            {
                verticalSpeed = value;
                NotifyPropertyChanged("VerticalSpeed");
            }
        }
        volatile string roll;
        public string Roll
        {
            get { return roll; }
            set
            {
                roll = value;
                NotifyPropertyChanged("Roll");
            }
        }
        volatile string pitch;
        public string Pitch
        {
            get { return pitch; }
            set
            {
                pitch = value;
                NotifyPropertyChanged("Pitch");
            }
        }
        volatile string heading;
        public string Heading
        {
            get { return heading; }
            set
            {
                heading = value;
                NotifyPropertyChanged("Heading");
            }
        }
        volatile string altitude;
        public string Altitude
        {
            get { return altitude; }
            set
            {
                altitude = value;
                NotifyPropertyChanged("Altitude");
            }
        }
        volatile string altimeter;
        public string Altimeter
        {
            get { return altimeter; }
            set
            {
                altimeter = value;
                NotifyPropertyChanged("Altimeter");
            }
        }

        // Map
        volatile string latitude;
        public string Latitude
        {
            get { return latitude; }
            set
            {
                latitude = value;
                NotifyPropertyChanged("Latitude");
            }
        }
        volatile string longitude;
        public string Longitude
        {
            get { return longitude; }
            set
            {
                longitude = value;
                NotifyPropertyChanged("Longitude");
            }
        }
        volatile string location;
        public string Location
        {
            get { return location; }
            set
            {
                location = value;
                NotifyPropertyChanged("Location");
            }
        }
        volatile string error;
        public string Error
        {
            get { return error; }
            set
            {
                error = value;
                NotifyPropertyChanged("Error");
            }
        }

        private const int BUFFER_SIZE = 1024;
        private const int MESSAGE_END_BYTE = '\n';
        TcpClient client;
        NetworkStream stream;
        Mutex mutex = new Mutex();
        volatile bool stop = false;

        public event PropertyChangedEventHandler PropertyChanged;

        public FlightModel()
        {
        }

        public void Connect(string ip, int port)
        {
            try
            {
                client = new TcpClient(ip, port);
                stream = client.GetStream();
                isConnect = true;
                stop = false;
                FetchDataFromServer();
            }
            catch (Exception ex)
            {
                Error = "Failed to connect to server. Reason was " + ex.Message;
            }
        }

        public void Disconnect()
        {
            stop = true;
            isConnect = false;
            stream.Close();
            client.Close();
        }

        void FetchDataFromServer()
        {
            new Thread(delegate ()
            {
                Thread.CurrentThread.IsBackground = true;
                while (!stop)
                {
                    AirSpeed = SendCmdToServer("get /instrumentation/airspeed-indicator/indicated-speed-kt");
                    GroundSpeed = SendCmdToServer("get /instrumentation/gps/indicated-ground-speed-kt");
                    VerticalSpeed = SendCmdToServer("get /instrumentation/gps/indicated-vertical-speed");
                    Roll = SendCmdToServer("get /instrumentation/attitude-indicator/internal-roll-deg");
                    Pitch = SendCmdToServer("get /instrumentation/attitude-indicator/internal-pitch-deg");
                    Heading = SendCmdToServer("get /instrumentation/heading-indicator/indicated-heading-deg");
                    Altitude = SendCmdToServer("get /instrumentation/gps/indicated-altitude-ft");
                    Altimeter = SendCmdToServer("get /instrumentation/altimeter/indicated-altitude-ft");

                    Latitude = SendCmdToServer("get /position/latitude-deg", false);
                    Longitude = SendCmdToServer("get /position/longitude-deg", false);
                    Location = Latitude + "," + Longitude;

                    Thread.Sleep(250);
                }
            }).Start();
        }

        public string SendCmdToServer(string data, bool roundResponse = true)
        {
            if (isConnect)
            {
                try
                {
                    mutex.WaitOne();
                    byte[] byteData = Encoding.ASCII.GetBytes(data + '\n');
                    stream.Write(byteData, 0, byteData.Length);
                    byte[] buffer = new byte[BUFFER_SIZE];
                    StringBuilder receivedData = new StringBuilder();

                    bool wasMessageEndFound = false;
                    do
                    {
                        stream.Read(buffer, 0, buffer.Length);

                        for (int i = 0; i < BUFFER_SIZE; i++)
                        {
                            if (buffer[i] == MESSAGE_END_BYTE)
                            {
                                wasMessageEndFound = true;
                                break;
                            }

                            receivedData.Append((char)buffer[i]);
                        }
                    }
                    while (!wasMessageEndFound);

                    mutex.ReleaseMutex();
                    try
                    {
                        if (roundResponse)
                        {
                            double value = Convert.ToDouble(receivedData.ToString());
                            return Math.Round(value, 2).ToString();
                        }
                        else
                        {
                            return receivedData.ToString();
                        }

                    }
                    catch (System.FormatException)
                    {
                        return "ERR";
                    }
                }
                catch (Exception ex)
                {
                    Error = "Failed to connect to server. Reason was " + ex.Message;
                    mutex.ReleaseMutex();
                    return "ERR";
                }
            }
            return "0";
        }

        // Commands
        void SetValue(double value, string valueName)
        {
            if (value > 1)
            {
                value = 1;
            }
            else if (value < -1)
            {
                value = -1;
            }
            try
            {
                new Thread(delegate ()
                {
                    Thread.CurrentThread.IsBackground = true;
                    string response = SendCmdToServer("set " + valueName + " " + value.ToString());

                    if (!(value == Convert.ToDouble(response)))
                    {
                        Error = "Error: Failed to set " + valueName + " to " + value.ToString();
                        //Console.WriteLine("Server response was: " + response);
                    }
                }).Start();
            }
            catch (Exception ex)
            {
                Error = "Failed to connect to server. Reason was " + ex.Message;
            }

        }
        public void setRudder(double value)
        {
            SetValue(value, "/controls/flight/rudder");
        }
        public void setElevator(double value)
        {
            SetValue(value, "/controls/flight/elevator");
        }
        public void setAileron(double value)
        {
            SetValue(value, "/controls/flight/aileron");
        }
        public void setThrottle(double value)
        {
            SetValue(value, "/controls/engines/current-engine/throttle");
        }

        public void NotifyPropertyChanged(String propName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

    }
}
