using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CoffeeConnect
{
    public interface IUpdateGUI
    {
        void UpdateGUI(string Command, string text);
        void UpdateConnectionState(int state, string text);
    }
}
