using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using Xamarin.Forms;
using System.ComponentModel;

namespace CoffeeConnect
{
    public class BindingVals : INotifyPropertyChanged
    {
        private string mySensorValue;
        public string MySensorValue
        {
            get => mySensorValue;
            set 
            {
                if (mySensorValue == value)
                    return;

                mySensorValue = value;
                OnPropertyChanged(nameof(mySensorValue));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
