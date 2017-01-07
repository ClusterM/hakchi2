using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.clusterrr.FelLib
{
    class AWFELMessage
    {
        public AWFELStandardRequest.RequestType Cmd;
        public UInt16 Tag;
        public UInt32 Address;
        public UInt32 Len;
        public UInt32 Flags;

        public AWFELMessage()
        {
        }

        public AWFELMessage(byte[] data)
        {
            Cmd = (AWFELStandardRequest.RequestType)(data[0] | (data[1] * 0x100));
            Tag = (UInt16)(data[2] | (data[3] * 0x100));
            Address = (UInt32)(data[4] | (data[5] * 0x100) | (data[6] * 0x10000) | (data[7] * 0x1000000));
            Len = (UInt32)(data[8] | (data[9] * 0x100) | (data[10] * 0x10000) | (data[11] * 0x1000000));
            Flags = (UInt32)(data[12] | (data[13] * 0x100) | (data[14] * 0x10000) | (data[15] * 0x1000000));
        }

        public byte[] Data
        {
            get
            {
                var data = new byte[16];
                data[0] = (byte)((UInt16)Cmd & 0xFF); // mark
                data[1] = (byte)(((UInt16)Cmd >> 8) & 0xFF); // mark
                data[2] = (byte)(Tag & 0xFF); // tag
                data[3] = (byte)((Tag >> 8) & 0xFF); // tag
                data[4] = (byte)(Address & 0xFF); // address
                data[5] = (byte)((Address >> 8) & 0xFF); // address
                data[6] = (byte)((Address >> 16) & 0xFF); // address
                data[7] = (byte)((Address >> 24) & 0xFF); // address
                data[8] = (byte)(Len & 0xFF); // len
                data[9] = (byte)((Len >> 8) & 0xFF); // len
                data[10] = (byte)((Len >> 16) & 0xFF); // len
                data[11] = (byte)((Len >> 24) & 0xFF); // len
                data[12] = (byte)(Flags & 0xFF); // flags
                data[13] = (byte)((Flags >> 8) & 0xFF); // flags
                data[14] = (byte)((Flags >> 16) & 0xFF); // flags
                data[15] = (byte)((Flags >> 24) & 0xFF); // flags
                
                return data;
            }
        }
    }
}
