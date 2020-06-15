using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeeConnect
{
    public interface IUpdateGUI
    {
        void UpdateGUI(string Command);
        void UpdateConnectionState(int state, string text);
    }
}
