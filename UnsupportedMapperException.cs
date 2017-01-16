using com.clusterrr.Famicom;
using System;

namespace com.clusterrr.hakchi_gui
{
    public class UnsupportedMapperException : Exception
    {
        public readonly NesFile ROM;
        public UnsupportedMapperException(NesFile nesFile)
        {
            ROM = nesFile;
        }
    }
}
