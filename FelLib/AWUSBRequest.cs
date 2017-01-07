using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.clusterrr.FelLib
{
    public class AWUSBRequest
    {
        public enum RequestType { AW_USB_READ = 0x11, AW_USB_WRITE = 0x12 };
        public UInt32 Tag = 0;
        public UInt32 Len;
        public RequestType Cmd;
        public byte CmdLen = 0x0C;
        
        public AWUSBRequest()
        {
        }

        public AWUSBRequest(byte[] data)
        {
            if (data[0] != 'A' || data[1] != 'W' || data[2] != 'U' || data[3] != 'C')
                throw new FelParseException();
            Tag = (UInt32)(data[4] | (data[5] * 0x100) | (data[6] * 0x10000) | (data[7] * 0x1000000));
            Len = (UInt32)(data[8] | (data[9] * 0x100) | (data[10] * 0x10000) | (data[11] * 0x1000000));
            CmdLen = data[15];
            Cmd = (RequestType)data[16];
        }

        public byte[] Data
        {
            get
            {
                var data = new byte[32];
                data[0] = (byte)'A';
                data[1] = (byte)'W';
                data[2] = (byte)'U';
                data[3] = (byte)'C';
                data[4] = (byte)(Tag & 0xFF); // tag
                data[5] = (byte)((Tag >> 8) & 0xFF); // tag
                data[6] = (byte)((Tag >> 16) & 0xFF); // tag
                data[7] = (byte)((Tag >> 24) & 0xFF); // tag
                data[8] = (byte)(Len & 0xFF); // len
                data[9] = (byte)((Len >> 8) & 0xFF); // len
                data[10] = (byte)((Len >> 16) & 0xFF); // len
                data[11] = (byte)((Len >> 24) & 0xFF); // len
                data[12] = data[13] = 0; // reserved1
                data[14] = 0; // reserved2
                data[15] = CmdLen; // cmd_len
                data[16] = (byte)Cmd;
                data[17] = 0; // reserved3
                data[18] = (byte)(Len & 0xFF); // len
                data[19] = (byte)((Len >> 8) & 0xFF); // len
                data[20] = (byte)((Len >> 16) & 0xFF); // len
                data[21] = (byte)((Len >> 24) & 0xFF); // len
                return data;
            }
        }
    }
}
