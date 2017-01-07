using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.clusterrr.FelLib
{
    public class AWFELStatusResponse
    {
        public UInt16 Mark = 0xFFFF;
        public UInt16 Tag = 0;
        public byte State;

        public AWFELStatusResponse()
        {
        }

        public AWFELStatusResponse(byte[] data)
        {
            Mark = (UInt16)(data[0] | (data[1] * 0x100));
            Tag = (UInt16)(data[2] | (data[3] * 0x100));
            State = data[4];
        }

        public byte[] Data
        {
            get
            {
                var data = new byte[8];
                data[0] = (byte)(Mark & 0xFF); // mark
                data[1] = (byte)((Mark >> 8) & 0xFF); // mark
                data[2] = (byte)(Tag & 0xFF); // tag
                data[3] = (byte)((Tag >> 8) & 0xFF); // tag
                data[4] = State;
                return data;
            }
        }
    }
}
