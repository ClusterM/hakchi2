using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace com.clusterrr.hakchi_gui
{
    interface ICloverAutofill
    {
        bool TryAutofill(uint crc32);
    }
}
