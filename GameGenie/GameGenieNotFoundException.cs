using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.clusterrr.hakchi_gui
{
    public class GameGenieNotFoundException : Exception
    {
        public readonly string Code;
        public GameGenieNotFoundException(string code)
            : base(string.Format("Invalid code \"{0}\"", code))
        {
            Code = code;
        }
    }
}
