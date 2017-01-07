/*  WinUSBNet library
 *  (C) 2010 Thomas Bleeker (www.madwizard.org)
 *  
 *  Licensed under the MIT license, see license.txt or:
 *  http://www.opensource.org/licenses/mit-license.php
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MadWizard.WinUSBNet
{

    /// <summary>
    /// Describes the policy for a specific USB pipe
    /// </summary>
    public class USBPipePolicy
    {

        private byte _pipeID;
        private int _interfaceIndex;
        private USBDevice _device;

        internal USBPipePolicy(USBDevice device, int interfaceIndex, byte pipeID)
        {
            _pipeID = pipeID;
            _interfaceIndex = interfaceIndex;
            _device = device;
        }

 

        private void RequireDirectionOut()
        {
            // Some policy types only apply specifically to OUT direction pipes
            if ((_pipeID & 0x80) != 0)
                throw new NotSupportedException("This policy type is only allowed on OUT direction pipes.");
        }

        private void RequireDirectionIn()
        {
            // Some policy types only apply specifically to IN  direction pipes
            // This function checks for this.
            if ((_pipeID & 0x80) == 0)
                throw new NotSupportedException("This policy type is only allowed on IN direction pipes.");
        }

        /// <summary>
        /// When false, read requests fail when the device returns more data than requested. When true, extra data is 
        /// saved and returned on the next read. Default value is true. Only available on IN direction pipes.
        /// </summary>
        /// <seealso href="http://msdn.microsoft.com/en-us/library/aa476439.aspx">WinUSB_GetPipePolicy for a more detailed description</seealso>
        public bool AllowPartialReads
        {
            get
            {
                RequireDirectionIn();
                return _device.InternalDevice.GetPipePolicyBool(_interfaceIndex, _pipeID, API.POLICY_TYPE.ALLOW_PARTIAL_READS);
            }
            set
            {
                RequireDirectionIn();
                _device.InternalDevice.SetPipePolicy(_interfaceIndex, _pipeID, API.POLICY_TYPE.ALLOW_PARTIAL_READS, value);
            }
        }

        /// <summary>
        /// When true, the driver fails stalled data transfers, but the driver clears the stall condition automatically. Default
        /// value is false.
        /// </summary>
        /// <seealso href="http://msdn.microsoft.com/en-us/library/aa476439.aspx">WinUSB_GetPipePolicy for a more detailed description</seealso>
        public bool AutoClearStall
        {
            get
            {
                return _device.InternalDevice.GetPipePolicyBool(_interfaceIndex, _pipeID, API.POLICY_TYPE.AUTO_CLEAR_STALL);
            }
            set
            {
                _device.InternalDevice.SetPipePolicy(_interfaceIndex, _pipeID, API.POLICY_TYPE.AUTO_CLEAR_STALL, value);
            }
        }
        
        /// <summary>
        /// If both AllowPartialReads and AutoFlush are true, when the device returns more data than requested by the client it
        /// will discard the remaining data. Default value is false. Only available on IN direction pipes.
        /// </summary>
        /// <seealso href="http://msdn.microsoft.com/en-us/library/aa476439.aspx">WinUSB_GetPipePolicy for a more detailed description</seealso>
        public bool AutoFlush
        {
            get
            {
                RequireDirectionIn();
                return _device.InternalDevice.GetPipePolicyBool(_interfaceIndex, _pipeID, API.POLICY_TYPE.AUTO_FLUSH); ;
            }
            set
            {
                RequireDirectionIn();
                _device.InternalDevice.SetPipePolicy(_interfaceIndex, _pipeID, API.POLICY_TYPE.AUTO_FLUSH, value);
            }
        }
        /// <summary>
        /// When true, read operations are completed only when the number of bytes requested by the client has been received. Default value is false.
        /// Only available on IN direction pipes.
        /// </summary>
        /// <seealso href="http://msdn.microsoft.com/en-us/library/aa476439.aspx">WinUSB_GetPipePolicy for a more detailed description</seealso>
        public bool IgnoreShortPackets
        {
            get
            {
                RequireDirectionIn();
                return _device.InternalDevice.GetPipePolicyBool(_interfaceIndex, _pipeID, API.POLICY_TYPE.IGNORE_SHORT_PACKETS); ;
            }
            set
            {
                RequireDirectionIn();
                _device.InternalDevice.SetPipePolicy(_interfaceIndex, _pipeID, API.POLICY_TYPE.IGNORE_SHORT_PACKETS, value);
            }
        }
   
        /// <summary>
        /// Specifies the timeout in milliseconds for pipe operations. If an operation does not finish within the specified time it will fail.
        /// When set to zero, no timeout is used. Default value is zero.
        /// </summary>
        /// <seealso href="http://msdn.microsoft.com/en-us/library/aa476439.aspx">WinUSB_GetPipePolicy for a more detailed description</seealso>
        public int PipeTransferTimeout
        {
            get
            {
                return (int)_device.InternalDevice.GetPipePolicyUInt(_interfaceIndex, _pipeID, API.POLICY_TYPE.PIPE_TRANSFER_TIMEOUT);
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Pipe transfer timeout cannot be negative.");
                _device.InternalDevice.SetPipePolicy(_interfaceIndex, _pipeID, API.POLICY_TYPE.PIPE_TRANSFER_TIMEOUT, (uint)value);
            }
        }

        /// <summary>
        /// When true, read and write operations to the pipe must have a buffer length that is a multiple of the maximum endpoint packet size, 
        /// and the length must be less than the maximum transfer size. With these conditions met, data is sent directly to the USB driver stack,
        /// bypassing the queuing and error handling of WinUSB. 
        /// Default value is false. 
        /// </summary>
        /// <seealso href="http://msdn.microsoft.com/en-us/library/aa476439.aspx">WinUSB_GetPipePolicy for a more detailed description</seealso>
        public bool RawIO
        {
            get
            {
                return _device.InternalDevice.GetPipePolicyBool(_interfaceIndex, _pipeID, API.POLICY_TYPE.RAW_IO); ;
            }
            set
            {
                _device.InternalDevice.SetPipePolicy(_interfaceIndex, _pipeID, API.POLICY_TYPE.RAW_IO, value);
            }
        }

        /// <summary>
        /// When true, every write request that is a multiple of the maximum packet size for the endpoint is terminated with a zero-length packet. 
        /// Default value is false. Only available on OUT direction pipes.
        /// </summary>
        /// <seealso href="http://msdn.microsoft.com/en-us/library/aa476439.aspx">WinUSB_GetPipePolicy for a more detailed description</seealso>
        public bool ShortPacketTerminate
        {
            get
            {
                RequireDirectionOut();
                return _device.InternalDevice.GetPipePolicyBool(_interfaceIndex, _pipeID, API.POLICY_TYPE.SHORT_PACKET_TERMINATE); ;
            }
            set
            {
                RequireDirectionOut();
                _device.InternalDevice.SetPipePolicy(_interfaceIndex, _pipeID, API.POLICY_TYPE.SHORT_PACKET_TERMINATE, value);
            }
        }

    }
}
