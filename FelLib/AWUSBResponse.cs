using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.clusterrr.FelLib
{
    class AWUSBResponse
    {
        public UInt32 Tag;
        public UInt32 Residue;
        public byte CswStatus;

        public AWUSBResponse()
        {
        }

        public AWUSBResponse(byte[] data)
        {
            if (data[0] != 'A' || data[1] != 'W' || data[2] != 'U' || data[3] != 'S')
                throw new FelParseException();
            Tag = (UInt32)(data[4] | (data[5] * 0x100) | (data[6] * 0x10000) | (data[7] * 0x1000000));
            Residue = (UInt32)(data[8] | (data[9] * 0x100) | (data[10] * 0x10000) | (data[11] * 0x1000000));
            CswStatus = data[12];
        }

        public byte[] Data
        {
            get
            {
                var data = new byte[13];
                data[0] = (byte)'A';
                data[1] = (byte)'W';
                data[2] = (byte)'U';
                data[3] = (byte)'S';
                data[4] = (byte)(Tag & 0xFF); // tag
                data[5] = (byte)((Tag >> 8) & 0xFF); // tag
                data[6] = (byte)((Tag >> 16) & 0xFF); // tag
                data[7] = (byte)((Tag >> 24) & 0xFF); // tag
                data[8] = (byte)(Residue & 0xFF); // residue
                data[9] = (byte)((Residue >> 8) & 0xFF); // residue
                data[10] = (byte)((Residue >> 16) & 0xFF); // residue
                data[11] = (byte)((Residue >> 24) & 0xFF); // residue
                data[12] = CswStatus; // csw_status
                return data;
            }
        }

    }
}
