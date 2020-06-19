using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeeConnect
{
    public interface IWifiConnect
    {
       void ConnectToWifi(string ip, string port);
       bool CheckValidIpAddress(string ip);
       bool CheckValidPort(string port);
       string executeCommand(string cmd);
    }
}
