using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.clusterrr.FelLib
{
    public class AWFELStandardRequest
    {
        public enum RequestType
        {
            FEL_VERIFY_DEVICE = 0x1, // (Read length 32 => AWFELVerifyDeviceResponse)
            FEL_SWITCH_ROLE = 0x2,
            FEL_IS_READY = 0x3, // (Read length 8)
            FEL_GET_CMD_SET_VER = 0x4,
            FEL_DISCONNECT = 0x10,
            FEL_DOWNLOAD = 0x101, // (Write data to the device)
            FEL_RUN = 0x102, // (Execute code)
            FEL_UPLOAD = 0x103, // (Read data from the device)        
        }
        public RequestType Cmd;
        public UInt16 Tag;

        public AWFELStandardRequest()
        {
        }

        public AWFELStandardRequest(byte[] data)
        {
            Cmd = (RequestType)(data[0] | (data[1] * 0x100));
            Tag = (UInt16)(data[2] | (data[3] * 0x100));
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
                return data;
            }
        }
    }
}
