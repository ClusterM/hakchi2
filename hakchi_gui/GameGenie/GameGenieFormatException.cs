using System;

namespace com.clusterrr.hakchi_gui
{
    public class GameGenieFormatException : Exception
    {
        public readonly string Code;
        public GameGenieFormatException(string code)
            : base(string.Format("Invalid code \"{0}\"", code))
        {
            Code = code;
        }
    }
}
