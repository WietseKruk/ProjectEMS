using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CoffeeConnect.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

[assembly: Dependency(typeof(ArduinoConnect))]

namespace CoffeeConnect.Droid
{
    public class ArduinoConnect : IWifiConnect
    {
        Socket socket = null;
        Timer timerSockets;
        BindingVals bv = new BindingVals();

        public void ConnectToWifi(string ip, string port)
        {
            //Only one command can be serviced in an timer tick, schedule from list
           timerSockets = new System.Timers.Timer() { Interval = 1000, Enabled = false }; // Interval >= 750
            timerSockets.Elapsed += (obj, args) =>
            {

                if (socket != null) // only if socket exists
                {
                    // Send a command to the Arduino server on every tick (loop though list)

                    string commandA = executeCommand("a");
                    //SetSensorValue(commandA);
                    DependencyService.Get<IUpdateGUI>().UpdateGUI(commandA);

                }
                else timerSockets.Enabled = false;  // If socket broken -> disable timer
            };


            if (socket == null) // create new socket
            {
                DependencyService.Get<IUpdateGUI>().UpdateConnectionState(1, "Connecting...");
                try  // to connect to the server (Arduino).
                {
                    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socket.Connect(new IPEndPoint(IPAddress.Parse(ip), Convert.ToInt32(port)));
                    if (socket.Connected)
                    {
                        DependencyService.Get<IUpdateGUI>().UpdateConnectionState(2, "Connected");
                        timerSockets.Enabled = true; //Activate timer for communication with Arduino     
                    }
                }
                catch (Exception exception)
                {
                    timerSockets.Enabled = false;
                    if (socket != null)
                    {
                        socket.Close();
                        socket = null;
                    }
                    DependencyService.Get<IUpdateGUI>().UpdateConnectionState(4, exception.Message);
                }
            }
            else // disconnect socket
            {
                socket.Close(); socket = null;
                timerSockets.Enabled = false;
                DependencyService.Get<IUpdateGUI>().UpdateConnectionState(4, "Disconnected");
            }
        }

        public string executeCommand(string cmd)
        {
            byte[] buffer = new byte[4]; // response is always 4 bytes
            int bytesRead = 0;
            string result = "---";

            if (socket != null)
            {
                //Send command to server
                socket.Send(Encoding.ASCII.GetBytes(cmd));

                try //Get response from server
                {
                    //Store received bytes (always 4 bytes, ends with \n)
                    bytesRead = socket.Receive(buffer);  // If no data is available for reading, the Receive method will block until data is available,
                    //Read available bytes.              // socket.Available gets the amount of data that has been received from the network and is available to be read
                    while (socket.Available > 0) bytesRead = socket.Receive(buffer);
                    if (bytesRead == 4)
                        result = Encoding.ASCII.GetString(buffer, 0, bytesRead - 1); // skip \n
                    else result = "err";
                }
                catch (Exception exception)
                {
                    result = exception.ToString();
                    if (socket != null)
                    {
                        socket.Close();
                        socket = null;
                    }
                    DependencyService.Get<IUpdateGUI>().UpdateConnectionState(3, result);
                }
            }
            return result;
        }

        //Check if the entered IP address is valid.
        public bool CheckValidIpAddress(string ip)
        {
            if (ip != "")
            {
                //Check user input against regex (check if IP address is not empty).
                Regex regex = new Regex("\\b((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)(\\.|$)){4}\\b");
                Match match = regex.Match(ip);
                return match.Success;
            }
            else return false;
        }

        //Check if the entered port is valid.
        public bool CheckValidPort(string port)
        {
            //Check if a value is entered.
            if (port != "")
            {
                Regex regex = new Regex("[0-9]+");
                Match match = regex.Match(port);

                if (match.Success)
                {
                    int portAsInteger = Int32.Parse(port);
                    //Check if port is in range.
                    return ((portAsInteger >= 0) && (portAsInteger <= 65535));
                }
                else return false;
            }
            else return false;
        }

        private void SetSensorValue(string value)
        {
            //var binding_context = (BindingContext)


            bv.MySensorValue = value;
        }
    }
}