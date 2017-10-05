using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.clusterrr.hakchi_gui
{
    interface ISupportsGameGenie
    {
        string GameGeniePath { get; }
        string GameGenie { get; set; }
        void ApplyGameGenie();
    }
}
