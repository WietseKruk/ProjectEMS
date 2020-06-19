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
    public partial class MainPage : ContentPage, IUpdateGUI
    {
        public BindingVals bv = new BindingVals();
        public int count;
        public DateTime alarm;
        public string sensortje = "geen sensorwaarde gevonden";
        


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
            alarm = new DateTime(2020, 6, 11, hours, min, 0);
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
                        DependencyService.Get<IWifiConnect>().executeCommand("Letter van command");
                    }
                    AlarmText.Text = "De koffie staat klaar!";
                    DependencyService.Get<IAudio>().PlayAudioFile("Alarm.mp3");
                    //waar is T???????????????????????????????????????????????????????????????????????????? bruh_moment_sound_effect_#3.mp3
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
            AlarmText.Text = Convert.ToString("Je koffie staat klaar om " + alarm.ToString("HH:mm"));

            if (now.Hour == alarm.Hour && now.Minute == alarm.Minute)
            {
                return true;
            }
            else return false;
        }

        public void UpdateGUI(string textview)
        {
            //if (result == "OFF")
            //{
            //    TempLabel.TextColor = Color.Red;
            //    bv.MySensorValue = "OFF";
            //}
            //else if (result == " ON")
            //{
                Device.BeginInvokeOnMainThread(() =>
                {
                    TempLabel.Text = textview;
                });


                //TempLabel.Text = textview;
                //bv.MySensorValue = textview;
            //}
            //else bv.MySensorValue = "";
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            //Validate the user input (IP address and port)
            if (DependencyService.Get<IWifiConnect>().CheckValidIpAddress(ipBox.Text) && DependencyService.Get<IWifiConnect>().CheckValidPort(portBox.Text))
            {
                DependencyService.Get<IWifiConnect>().ConnectToWifi(ipBox.Text, portBox.Text);
            }
        }

        public void UpdateConnectionState(int state, string text)
        {
            // connectButton
            string butConText = "Connect";  // default text
            bool butConEnabled = true;      // default state
            Color color = Color.Red;        // default color
            // pinButton

            //Set "Connect" button label according to connection state.
            if (state == 1)
            {
                butConText = "Please wait";
                color = Color.Orange;
                butConEnabled = false;
            }
            else
            if (state == 2)
            {
                butConText = "Disconnect";
                color = Color.Green;
            }

            if (butConText != null)  // text existst
            {
                ButConn.Text = butConText;
                ButConn.IsVisible = butConEnabled;
            }
        }

        public void SensorUpdate(string sensorVal)
        {
            TempLabel.Text = sensorVal;
        }
        private void Koffieswitch(object sender, EventArgs e)
        {
           
        }
    }
}
