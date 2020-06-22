using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Timers;
using Xamarin.Forms.PlatformConfiguration;
using CoffeeConnect;
using System.Collections.ObjectModel;

[assembly: Dependency(typeof(MainPage))]

namespace CoffeeConnect
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public BindingVals bv = new BindingVals();
        public int count;
        public DateTime alarm;
        public string sensortje = "geen sensorwaarde gevonden";
        public bool connectedCheck = false;
        

        public MainPage()
        {
            InitializeComponent();
            ShowTime();
            TimeCompare();

            //DependencyService.Get<IWifiConnect>().ConnectToWifi(ip, port);
        }

        public void ShowTime()
        {
            //Laat de tijd van nu zien
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                Device.BeginInvokeOnMainThread(() =>
                Timelbl.Text = DateTime.Now.ToString("HH:mm:ss")
                );
                return true;
            });
        }

         private void StartAlarm_Clicked(object sender, EventArgs e)
        {
            int hours = Convert.ToInt32(HoursBox.Text);
            int min = Convert.ToInt32(MinutesBox.Text);
            if (min == 00)
            {
                hours -= 1;
                min += 59;
            }
            else
            {
                min = min - 1;
            }
            alarm = new DateTime(2020, 6, 11, hours, min, 50);
            StartAlarm.IsVisible = false;
            StopAlarm.IsVisible = true;
            SetAlarmTime.IsVisible = false;
            AlarmText.IsVisible = true;
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                bool CheckTime = TimeCompare();
                if (CheckTime)
                {
                    if(xamlSwitch.IsToggled == true)
                    {
                        DependencyService.Get<IWifiConnect>().executeCommand("t");
                    }
                    AlarmText.Text = "De koffie staat klaar!";
                    DependencyService.Get<IAudio>().PlayAudioFile("Alarm.mp3");
                    return false;
                }
                else
                    return true;

            });
        }

        public void StopAlarm_OFF(object sender, EventArgs e)
        {
            DependencyService.Get<IAudio>().StopAudio();
            StopAlarm.IsVisible = false;
            StartAlarm.IsVisible = true;
            SetAlarmTime.IsVisible = true;
            AlarmText.IsVisible = false;
        }

        public bool TimeCompare()
        {

            DateTime now = DateTime.Now;
            if(xamlSwitch.IsToggled == false)
            {
                AlarmText.Text = Convert.ToString("Je wekker gaat af zonder koffie om " + alarm.ToString("HH:mm"));
            }else
            AlarmText.Text = Convert.ToString("Je koffie staat klaar om " + alarm.ToString("HH:mm"));

            if (now.Hour == alarm.Hour && now.Minute == alarm.Minute && now.Second == alarm.Second)
            {
                return true;
            }
            else return false;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            //Validate the user input (IP address and port)
            if (DependencyService.Get<IWifiConnect>().CheckValidIpAddress(ipBox.Text) && DependencyService.Get<IWifiConnect>().CheckValidPort(portBox.Text))
            {
                try
                {
                    DependencyService.Get<IWifiConnect>().ConnectToWifi(ipBox.Text, portBox.Text);
                    connectedCheck = true;
                }catch
                {
                    connectedCheck = false;
                }

                if (connectedCheck == true)
                {
                    IsConnected.IsVisible = false;
                    notConnected.IsVisible = true;
                }

                else
                    IsConnected.IsVisible = true;
                    notConnected.IsVisible = true;
            }
        }

        private void Disconnect_Button_Clicked(object sender, EventArgs e)
        {
            notConnected.IsVisible = false;
            IsConnected.IsVisible = true;
            connectedCheck = false;
        }

        private void Koffieswitch(object sender, EventArgs e)
        {

        }

    }
}
