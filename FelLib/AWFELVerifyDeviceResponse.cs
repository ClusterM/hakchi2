using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.clusterrr.FelLib
{
    public class AWFELVerifyDeviceResponse
    {
        public UInt32 Board;
        public UInt32 FW;
        public UInt16 Mode;
        public byte DataFlag;
        public byte DataLength;
        public UInt32 DataStartAddress;

        public AWFELVerifyDeviceResponse()
        {
        }

        public AWFELVerifyDeviceResponse(byte[] data)
        {
            if (data[0] != 'A' || data[1] != 'W' || data[2] != 'U' || data[3] != 'S'
                || data[4] != 'B' || data[5] != 'F' || data[6] != 'E' || data[7] != 'X')
                throw new FelParseException();
            Board = (UInt32)(data[8] | (data[9] * 0x100) | (data[10] * 0x10000) | (data[11] * 0x1000000));
            FW = (UInt32)(data[12] | (data[13] * 0x100) | (data[14] * 0x10000) | (data[15] * 0x1000000));
            Mode = (UInt16)(data[16] | (data[17] * 0x100));
            DataFlag = data[18];
            DataLength = data[19];
            DataStartAddress = (UInt32)(data[20] | (data[21] * 0x100) | (data[22] * 0x10000) | (data[23] * 0x1000000));
        }

        public byte[] Data
        {
            get
            {
                var data = new byte[32];
                data[0] = (byte)'A';
                data[1] = (byte)'W';
                data[2] = (byte)'U';
                data[3] = (byte)'S';
                data[4] = (byte)'B';
                data[5] = (byte)'F';
                data[6] = (byte)'E';
                data[7] = (byte)'X';
                data[8] = (byte)(Board & 0xFF); // board
                data[9] = (byte)((Board >> 8) & 0xFF); // board
                data[10] = (byte)((Board >> 16) & 0xFF); // board
                data[11] = (byte)((Board >> 24) & 0xFF); // board
                data[12] = (byte)(FW & 0xFF); // fw
                data[13] = (byte)((FW >> 8) & 0xFF); // fw
                data[14] = (byte)((FW >> 16) & 0xFF); // fw
                data[15] = (byte)((FW >> 24) & 0xFF); // fw
                data[16] = (byte)(Mode & 0xFF); // mode
                data[17] = (byte)((Mode >> 8) & 0xFF); // mode
                data[18] = DataFlag;
                data[19] = DataLength;
                data[20] = (byte)(DataStartAddress & 0xFF); // data_start_address
                data[21] = (byte)((DataStartAddress >> 8) & 0xFF); // data_start_address
                data[22] = (byte)((DataStartAddress >> 16) & 0xFF); // data_start_address
                data[23] = (byte)((DataStartAddress >> 24) & 0xFF); // data_start_address
                return data;
            }
        }
    }
}
