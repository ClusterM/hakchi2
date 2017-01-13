using com.clusterrr.Famicom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.clusterrr.hakchi_gui
{
    public class UnsupportedFourScreenException : Exception
    {
        public readonly NesFile ROM;
        public UnsupportedFourScreenException(NesFile nesFile)
        {
            ROM = nesFile;
        }
    }
}
