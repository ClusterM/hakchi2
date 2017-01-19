using MadWizard.WinUSBNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace com.clusterrr.FelLib
{
    public class Fel
    {
        public byte[] Fes1Bin;
        byte[] uBootBin;

        public enum CurrentAction { RunningCommand, ReadingMemory, WritingMemory }
        public delegate void OnFelProgress(CurrentAction action, string command);

        USBDevice device = null;
        byte inEndp = 0;
        byte outEndp = 0;
        const int ReadTimeout = 1000;
        const int WriteTimeout = 1000;
        public const int MaxBulkSize = 0x10000;
        UInt16 vid, pid;
        bool DramInitDone = false;

        int cmdOffset = -1;
        public const UInt32 fes1_base_m = 0x2000;
        public const UInt32 dram_base = 0x40000000;
        public const UInt32 flash_mem_base = 0x43800000;
        public const UInt32 flash_mem_size = 0x20;
        public const UInt32 uboot_base_m = 0x47000000u;
        public const UInt32 sector_size = 0x20000;
        public const UInt32 uboot_base_f = 0x100000;
        public const UInt32 kernel_base_f = (sector_size * 0x30);
        public const UInt32 kernel_base_m = flash_mem_base;
        public const UInt32 kernel_max_size = (uboot_base_m - flash_mem_base);
        public const UInt32 kernel_max_flash_size = (sector_size * 0x20);
        const string fastboot = "fastboot_test";

        public byte[] UBootBin
        {
            get
            {
                return uBootBin;
            }

            set
            {
                uBootBin = value;
                var prefix = "bootcmd=";
                for (int i = 0; i < uBootBin.Length - prefix.Length; i++)
                    if (Encoding.ASCII.GetString(uBootBin, i, prefix.Length) == prefix)
                    {
                        cmdOffset = i + prefix.Length;
                        break;
                    }
            }
        }

        public static bool DeviceExists(UInt16 vid, UInt16 pid)
        {
            var fel = new Fel();
            try
            {
                fel.Open(vid, pid);
                Debug.WriteLine("Device detection successful");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Device detection error: " + ex.Message + ex.StackTrace);
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
            Debug.WriteLine("Trying to open device...");
            device = USBDevice.GetSingleDevice(vid, pid);
            if (device == null) throw new FelException("Device with such VID and PID not found");
            Debug.WriteLine("Checking USB endpoints...");
            foreach (var pipe in device.Pipes)
            {
                if (pipe.IsIn)
                {
                    inEndp = pipe.Address;
                    Debug.WriteLine("IN endpoint found: " + inEndp);
                }
                else
                {
                    outEndp = pipe.Address;
                    Debug.WriteLine("Out endpoint found: " + outEndp);
                }
            }
            device.Pipes[inEndp].Policy.PipeTransferTimeout = ReadTimeout;
            device.Pipes[outEndp].Policy.PipeTransferTimeout = WriteTimeout;
            Debug.WriteLine("Trying to verify device");
            if (VerifyDevice().Board != 0x00166700) throw new FelException("Invalid board ID: " + VerifyDevice().Board);
        }
        public void Close()
        {
            if (device != null)
            {
                try
                {
                    device.Pipes[inEndp].Abort();
                }
                catch { }
                try
                {
                    device.Pipes[outEndp].Abort();
                }
                catch
                {
                }
                device.Dispose();
                device = null;
            }
        }

        private void WriteToUSB(byte[] buffer)
        {
            Debug.WriteLine("-> " + BitConverter.ToString(buffer));
            device.Pipes[outEndp].Write(buffer);
        }

        private int ReadFromUSB(byte[] buffer, int offset, int length)
        {
            var data = device.Pipes[inEndp].Read(buffer, offset, length);
            Debug.WriteLine("<- " + BitConverter.ToString(buffer));
            return data;
        }
        private byte[] ReadFromUSB(UInt32 length)
        {
            var result = new byte[length];
            int pos = 0;
            while (pos < length)
            {
                pos += ReadFromUSB(result, pos, (int)(length - pos));
            }
            return result;
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

        private byte[] FelRead(UInt32 length)
        {
            var req = new AWUSBRequest();
            req.Cmd = AWUSBRequest.RequestType.AW_USB_READ;
            req.Len = length;
            WriteToUSB(req.Data);

            var result = ReadFromUSB(length);
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
            byte[] resp;
            try
            {
                resp = FelRead(32);
            }
            catch
            {
                resp = new byte[32];
            }
            var status = new AWFELStatusResponse(FelRead(8));
            return new AWFELVerifyDeviceResponse(resp);
        }

        public void WriteMemory(UInt32 address, byte[] buffer, OnFelProgress callback = null)
        {
            if (address >= dram_base)
                InitDram();

            UInt32 length = (UInt32)buffer.Length;
            if (length != (length & ~((UInt32)3)))
            {
                length = (length + 3) & ~((UInt32)3);
                var newBuffer = new byte[length];
                Array.Copy(buffer, 0, newBuffer, 0, buffer.Length);
                buffer = newBuffer;
            }

            int pos = 0;
            while (pos < buffer.Length)
            {
                if (callback != null) callback(CurrentAction.WritingMemory, null);
                var buf = new byte[Math.Min(buffer.Length - pos, MaxBulkSize)];
                Array.Copy(buffer, pos, buf, 0, buf.Length);
                FelRequest(AWFELStandardRequest.RequestType.FEL_DOWNLOAD, (UInt32)(address + pos), (uint)buf.Length);
                FelWrite(buf);
                var status = new AWFELStatusResponse(FelRead(8));
                if (status.State != 0) throw new FelException("FEL write error");
                pos += buf.Length;
            }
        }

        private byte[] ReadMemory(UInt32 address, UInt32 length, OnFelProgress callback = null)
        {
            if (address >= dram_base)
                InitDram();

            length = (length + 3) & ~((UInt32)3);

            var result = new List<byte>();
            while (length > 0)
            {
                if (callback != null) callback(CurrentAction.ReadingMemory, null);
                var l = Math.Min(length, MaxBulkSize);
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

        public bool InitDram(bool force = false)
        {
            if (DramInitDone && !force) return true;
            if (DramInitDone) return true;
            const UInt32 testSize = 0x80;
            if (Fes1Bin == null || Fes1Bin.Length < testSize)
                throw new FelException("Can't init DRAM, incorrect Fes1 binary");
            var buf = ReadMemory((UInt32)(fes1_base_m + Fes1Bin.Length - testSize), testSize);
            var buf2 = new byte[testSize];
            Array.Copy(Fes1Bin, Fes1Bin.Length - buf.Length, buf2, 0, testSize);
            if (buf.SequenceEqual(buf2))
            {
                return DramInitDone = true;
            }
            WriteMemory(fes1_base_m, Fes1Bin);
            Exec(fes1_base_m);
            Thread.Sleep(2000);
            return DramInitDone = true;
        }

        public byte[] ReadFlash(UInt32 address, UInt32 length, OnFelProgress callback = null)
        {
            var result = new List<byte>();
            while (((length + address % sector_size + sector_size - 1) / sector_size) > flash_mem_size)
            {
                var sectors = (length + address % sector_size + sector_size - 1) / sector_size - flash_mem_size;
                var buf = ReadFlash(address, sectors * sector_size - address % sector_size, callback);
                address += (uint)buf.Length;
                length -= (uint)buf.Length;
                result.AddRange(buf);
            }
            if (result.Count > 0) return result.ToArray();
            var command = string.Format("sunxi_flash phy_read {0:x} {1:x} {2:x};{3}", flash_mem_base, address / sector_size, (length + address % sector_size + sector_size - 1) / sector_size, fastboot);
            RunUbootCmd(command, false, callback);
            result.AddRange(ReadMemory(flash_mem_base + address % sector_size, length, callback));
            return result.ToArray();
        }

        public void WriteFlash(UInt32 address, byte[] buffer, OnFelProgress callback = null)
        {
            int length = buffer.Length;
            int pos = 0;
            if ((address % sector_size) != 0)
                throw new FelException(string.Format("Invalid address to flash: 0x{0:X8}", address));
            if ((length % sector_size) != 0)
                throw new FelException(string.Format("Invalid length to flash: 0x{0:X8}", length));
            byte[] newBuf;
            while ((length / sector_size) > flash_mem_size)
            {
                var sectors = (length / sector_size) - flash_mem_size;
                newBuf = new byte[sectors * sector_size];
                Array.Copy(buffer, pos, newBuf, 0, newBuf.Length);
                WriteFlash(address, newBuf, callback);
                address += (UInt32)newBuf.Length;
                length -= newBuf.Length;
                pos += newBuf.Length;
            }
            newBuf = new byte[length - pos];
            Array.Copy(buffer, pos, newBuf, 0, newBuf.Length);
            WriteMemory(flash_mem_base, newBuf, callback);
            var command = string.Format("sunxi_flash phy_write {0:x} {1:x} {2:x};{3}", flash_mem_base, address / sector_size, length / sector_size, fastboot);
            RunUbootCmd(command, false, callback);
        }

        public void Exec(UInt32 address)
        {
            FelRequest(AWFELStandardRequest.RequestType.FEL_RUN, address, 0);
            var status = new AWFELStatusResponse(FelRead(8));
            if (status.State != 0) throw new FelException("FEL run error");
        }

        public void RunUbootCmd(string command, bool noreturn = false, OnFelProgress callback = null)
        {
            if (callback != null) callback(CurrentAction.RunningCommand, command);
            if (cmdOffset < 0) throw new Exception("Invalid Unoot binary, command variable not found");
            const UInt32 testSize = 0x20;
            if (UBootBin == null || UBootBin.Length < testSize)
                throw new FelException("Can't init Uboot, incorrect Uboot binary");
            var buf = ReadMemory(uboot_base_m, testSize);
            var buf2 = new byte[testSize];
            Array.Copy(UBootBin, 0, buf2, 0, testSize);
            if (!buf.SequenceEqual(buf2))
                WriteMemory(uboot_base_m, UBootBin);
            var cmdBuff = Encoding.ASCII.GetBytes(command + "\0");
            WriteMemory((uint)(uboot_base_m + cmdOffset), cmdBuff);
            Exec((uint)uboot_base_m);
            if (noreturn) return;
            Close();
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(1000);
                if (callback != null) callback(CurrentAction.RunningCommand, command);
            }
            int errorCount = 0;
            while (true)
            {
                try
                {
                    Open(vid, pid);
                    break;
                }
                catch (Exception ex)
                {
                    errorCount++;
                    if (errorCount >= 10)
                    {
                        Close();
                        throw ex;
                    }
                    Thread.Sleep(2000);
                }
            }
        }
    }
}
