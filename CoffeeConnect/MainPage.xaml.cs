﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Timers;

namespace CoffeeConnect
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public int count;
        public DateTime alarm;
        public MainPage()
        {
            InitializeComponent();
            ShowTime();
            TimeCompare();
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
            AlarmText.Text = Convert.ToString("Je koffie staat klaar om " + alarm.ToString("HH:mm"));

            if (now.Hour == alarm.Hour && now.Minute == alarm.Minute)
            {
                return true;
            }
            else return false;

        }
    }
}
