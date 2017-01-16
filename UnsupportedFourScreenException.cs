using com.clusterrr.Famicom;
using System;

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
