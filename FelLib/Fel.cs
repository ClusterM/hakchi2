using MadWizard.WinUSBNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace com.clusterrr.FelLib
{
    public class Fel
    {
        USBDevice device = null;
        byte inEndp = 0;
        byte outEndp = 0;
        const int ReadTimeout = 500;
        public const int MaxBulkSize = 0x10000;
        UInt16 vid, pid;

        public static bool DeviceExists(UInt16 vid, UInt16 pid)
        {
            var fel = new Fel();                
            try
            {
                fel.Open(vid, pid);
                fel.VerifyDevice();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                fel.Close();
            }
        }

        public void Open(UInt16 vid, UInt16 pid)
        {
            this.vid = vid;
            this.pid = pid;
            Close();
            device = USBDevice.GetSingleDevice(vid, pid);
            if (device == null) throw new Exception("Device not found");
            foreach (var pipe in device.Pipes)
            {
                if (pipe.IsIn)
                    inEndp = pipe.Address;
                else
                    outEndp = pipe.Address;
            }
            device.Pipes[outEndp].Policy.PipeTransferTimeout = ReadTimeout;
            ClearInputBuffer();
        }
        public void Close()
        {
            if (device != null)
            {
                device.Dispose();
                device = null;
            }
        }

        public void ClearInputBuffer()
        {
            var dummyBuff = new byte[64];
            device.Pipes[inEndp].Policy.PipeTransferTimeout = 50;
            try
            {
                while (true) device.Pipes[inEndp].Read(dummyBuff);
            }
            catch { }
            device.Pipes[inEndp].Policy.PipeTransferTimeout = ReadTimeout;
        }

        private void WriteToUSB(byte[] buffer)
        {
            device.Pipes[outEndp].Write(buffer);
        }

        private byte[] ReadFromUSB(UInt32 len)
        {
            var result = new List<byte>();
            while (result.Count < len)
            {
                var buffer = new byte[len - result.Count];
                var l = device.Pipes[inEndp].Read(buffer);
                for (int i = 0; i < l && result.Count < len; i++)
                    result.Add(buffer[i]);
            }
            return result.ToArray();
        }

        private void FelWrite(byte[] buffer)
        {
            var req = new AWUSBRequest();
            req.Cmd = AWUSBRequest.RequestType.AW_USB_WRITE;
            req.Len = (uint)buffer.Length;
            WriteToUSB(req.Data);
            WriteToUSB(buffer);
            var resp = new AWUSBResponse(ReadFromUSB(13));
            if (resp.CswStatus != 0) throw new FelException("FEL write error");
        }

        private byte[] FelRead(UInt32 len)
        {
            var req = new AWUSBRequest();
            req.Cmd = AWUSBRequest.RequestType.AW_USB_READ;
            req.Len = len;
            WriteToUSB(req.Data);

            var result = ReadFromUSB(len);
            var resp = new AWUSBResponse(ReadFromUSB(13));
            if (resp.CswStatus != 0) throw new FelException("FEL read error");
            return result;
        }

        private void FelRequest(AWFELStandardRequest.RequestType command)
        {
            var req = new AWFELStandardRequest();
            req.Cmd = command;
            FelWrite(req.Data);
        }

        private void FelRequest(AWFELStandardRequest.RequestType command, UInt32 address, UInt32 length)
        {
            var req = new AWFELMessage();
            req.Cmd = command;
            req.Address = address;
            req.Len = length;
            FelWrite(req.Data);
        }

        public AWFELVerifyDeviceResponse VerifyDevice()
        {
            FelRequest(AWFELStandardRequest.RequestType.FEL_VERIFY_DEVICE);
            var resp = FelRead(32);
            var status = new AWFELStatusResponse(FelRead(8));
            return new AWFELVerifyDeviceResponse(resp);
        }

        public void WriteMemory(UInt32 address, byte[] buffer)
        {
            int pos = 0;
            while (pos < buffer.Length)
            {
                var buf = new byte[Math.Min(buffer.Length - pos, MaxBulkSize)];
                Array.Copy(buffer, pos, buf, 0, buf.Length);

                FelRequest(AWFELStandardRequest.RequestType.FEL_DOWNLOAD, (UInt32)(address + pos), (uint)buf.Length);
                FelWrite(buf);
                var status = new AWFELStatusResponse(FelRead(8));
                if (status.State != 0) throw new FelException("FEL write error");
                pos += buf.Length;
            }
        }

        public byte[] ReadMemory(UInt32 address, UInt32 length)
        {
            var result = new List<byte>();
            while (length > 0)
            {
                var l = Math.Min(length, MaxBulkSize);
                Console.WriteLine("Reading {0:X8}, size: {1:X8}...", address, l);
                FelRequest(AWFELStandardRequest.RequestType.FEL_UPLOAD, address, l);
                var r = FelRead((UInt32)l);
                result.AddRange(r);
                var status = new AWFELStatusResponse(FelRead(8));
                if (status.State != 0) throw new FelException("FEL read error");
                length -= l;
                address += l;
            }
            return result.ToArray();
        }

        public void Exec(UInt32 address, int pause = 0)
        {
            FelRequest(AWFELStandardRequest.RequestType.FEL_RUN, address, 0);
            var status = new AWFELStatusResponse(FelRead(8));
            if (status.State != 0) throw new FelException("FEL run error");
            //Close();
            Thread.Sleep(pause * 1000);
            //int errorCount = 0;
            //while (true)
            //{
            //    try
            //    {
            //        Open(vid, pid);
            //        return;
            //    }
            //    catch (Exception ex)
            //    {
            //        errorCount++;
            //        if (errorCount >= 8)
            //            throw ex;
            //    }
            //}
        }
    }
}
