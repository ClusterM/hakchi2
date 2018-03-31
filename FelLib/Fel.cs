using LibUsbDotNet;
using LibUsbDotNet.Main;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace com.clusterrr.FelLib
{
    public class Fel : IDisposable
    {
        public enum UbootType { Normal, SD }
        public struct VidPidPair
        {
            public UInt16 vid, pid;
            public VidPidPair(UInt16 vid, UInt16 pid)
            {
                this.vid = vid;
                this.pid = pid;
            }
        }
        public byte[] Fes1Bin;
        byte[] uBootBin;

        public enum CurrentAction { RunningCommand, ReadingMemory, WritingMemory }
        public delegate void OnFelProgress(CurrentAction action, string command);

        UsbDevice device = null;
        UsbEndpointReader epReader = null;
        UsbEndpointWriter epWriter = null;
        const int ReadTimeout = 1000;
        const int WriteTimeout = 1000;
        public const int MaxBulkSize = 0x10000;
        UInt16 vid, pid;
        bool DramInitDone = false;

        int cmdOffset = -1;
        public const UInt32 fes1_base_m = 0x2000;
        public const UInt32 dram_base = 0x40000000;
        public const UInt32 uboot_base_m = dram_base + 0x7000000u;
        public const UInt32 uboot_base_f = 0x100000u;
        public const UInt32 sector_size = 0x20000u;
        public const UInt32 uboot_maxsize_f = (sector_size * 0x10);
        public const UInt32 kernel_base_f = sector_size * 0x30;
        public const UInt32 kernel_max_size = sector_size * 0x20;
        public const UInt32 transfer_base_m = dram_base + 0x7400000u;
        public const UInt32 transfer_max_size = sector_size * 0x100;
        const string fastboot = "efex_test";

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
                if (fel.Open(vid, pid))
                {
                    Debug.WriteLine("Device detection successful");
                    return true;
                }
                else
                {
                    return false;
                }
            }
            finally
            {
                fel.Close();
            }
        }

        public bool Open(UInt16 vid, UInt16 pid)
        {
            try
            {
                this.vid = vid;
                this.pid = pid;
                Close();
                //Debug.WriteLine("Trying to open device...");
                var devices = UsbDevice.AllDevices;
                device = null;
                foreach (UsbRegistry regDevice in devices)
                {
                    if (regDevice.Vid == vid && regDevice.Pid == pid)
                    {
                        regDevice.Open(out device);
                        break;
                    }
                }
                if (device == null)
                {
#if VERY_DEBUG
                Debug.WriteLine("Device with such VID and PID not found");
#endif
                    return false;
                }

                IUsbDevice wholeUsbDevice = device as IUsbDevice;
                if (!ReferenceEquals(wholeUsbDevice, null))
                {
                    // This is a "whole" USB device. Before it can be used, 
                    // the desired configuration and interface must be selected.

                    // Select config #1
                    wholeUsbDevice.SetConfiguration(1);

                    // Claim interface #0.
                    wholeUsbDevice.ClaimInterface(0);
                }

                int inEndp = -1;
                int outEndp = -1;
                int inMax = 0;
                int outMax = 0;
                Debug.WriteLine("Checking USB endpoints...");
                foreach (var config in device.Configs)
                    foreach (var @interface in config.InterfaceInfoList)
                        foreach (var endp in @interface.EndpointInfoList)
                        {
                            if ((endp.Descriptor.EndpointID & 0x80) != 0)
                            {
                                inEndp = endp.Descriptor.EndpointID;
                                inMax = endp.Descriptor.MaxPacketSize;
                                Debug.WriteLine("IN endpoint found: " + inEndp);
                                Debug.WriteLine("IN endpoint maxsize: " + inMax);
                            }
                            else
                            {
                                outEndp = endp.Descriptor.EndpointID;
                                outMax = endp.Descriptor.MaxPacketSize;
                                Debug.WriteLine("OUT endpoint found: " + outEndp);
                                Debug.WriteLine("OUT endpoint maxsize: " + outMax);
                            }
                        }
                if (inEndp != 0x82 || outEndp != 0x01)
                {
                    Debug.WriteLine("Uncorrect FEL device/mode");
                    return false;
                }
                epReader = device.OpenEndpointReader((ReadEndpointID)inEndp, 65536);
                epWriter = device.OpenEndpointWriter((WriteEndpointID)outEndp);

                Debug.WriteLine("Trying to verify device");
                if (VerifyDevice().Board != 0x00166700)
                {
                    Debug.WriteLine("Invalid board ID: " + VerifyDevice().Board);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message + ex.StackTrace);
                return false;
            }
        }
        public void Close()
        {
            if (device != null)
                device.Close();
            device = null;
            if (epReader != null)
                epReader.Dispose();
            epReader = null;
            if (epWriter != null)
                epWriter.Dispose();
            epWriter = null;
        }

        private void WriteToUSB(byte[] buffer)
        {
#if VERY_DEBUG
            Debug.WriteLine("->[FEL] " + BitConverter.ToString(buffer));
#endif
            Debug.WriteLine(string.Format("-> {0} bytes", buffer.Length));
            int pos = 0;
            int l;
            while (pos < buffer.Length)
            {
                epWriter.Write(buffer, pos, buffer.Length - pos, WriteTimeout, out l);
                if (l > 0)
                    pos += l;
                else
                    throw new Exception("Can't write to USB");
            }
        }

        private int ReadFromUSB(byte[] buffer, int offset, int length)
        {
            int l;
            var result = epReader.Read(buffer, offset, length, ReadTimeout, out l);
            if (result != ErrorCode.Ok)
                throw new Exception("USB read error: " + result.ToString());
#if VERY_DEBUG
            Debug.WriteLine("<-[FEL] " + BitConverter.ToString(buffer));
#endif
            Debug.WriteLine(string.Format("<- {0} bytes", length));
            return l;
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
                callback?.Invoke(CurrentAction.WritingMemory, null);
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
                callback?.Invoke(CurrentAction.ReadingMemory, null);
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
            string command;
            if ((address % sector_size) != 0)
                throw new FelException(string.Format("Invalid flash address : 0x{0:X8}", address));
            if ((length % sector_size) != 0)
                throw new FelException(string.Format("Invalid flash length: 0x{0:X8}", length));
            while (length > 0)
            {
                var reqLen = Math.Min(length, transfer_max_size);
                command = string.Format("sunxi_flash phy_read {0:x} {1:x} {2:x};{3}", transfer_base_m, address / sector_size, (int)Math.Floor((double)reqLen / (double)sector_size), fastboot);
                RunUbootCmd(command, false, callback);
                var buf = ReadMemory(transfer_base_m + address % sector_size, reqLen, callback);
                result.AddRange(buf);
                address += (uint)buf.Length;
                length -= (uint)buf.Length;
            }
            return result.ToArray();
        }

        public void WriteFlash(UInt32 address, byte[] buffer, OnFelProgress callback = null)
        {
            var length = (uint)buffer.Length;
            uint pos = 0;
            if ((address % sector_size) != 0)
                throw new FelException(string.Format("Invalid flash address : 0x{0:X8}", address));
            if ((length % sector_size) != 0)
                throw new FelException(string.Format("Invalid flash length: 0x{0:X8}", length));
            while (length > 0)
            {
                var wrLength = Math.Min(length, transfer_max_size / 8);
                var newBuf = new byte[wrLength];
                Array.Copy(buffer, pos, newBuf, 0, wrLength);
                WriteMemory(transfer_base_m, newBuf, callback);
                var command = string.Format("sunxi_flash phy_write {0:x} {1:x} {2:x};{3}", transfer_base_m, address / sector_size, (int)Math.Floor((double)wrLength / (double)sector_size), fastboot);
                RunUbootCmd(command, false, callback);
                pos += (uint)wrLength;
                address += (uint)wrLength;
                length -= (uint)wrLength;
            }
        }

        public void Exec(UInt32 address)
        {
            FelRequest(AWFELStandardRequest.RequestType.FEL_RUN, address, 0);
            var status = new AWFELStatusResponse(FelRead(8));
            if (status.State != 0) throw new FelException("FEL run error");
        }

        public void RunUbootCmd(string command, bool noreturn = false, OnFelProgress callback = null)
        {
            callback?.Invoke(CurrentAction.RunningCommand, command);
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
                Thread.Sleep(500);
                callback?.Invoke(CurrentAction.RunningCommand, command);
            }
            int errorCount = 0;
            while (true)
            {
                if (!Open(vid, pid))
                {
                    errorCount++;
                    if (errorCount >= 10)
                    {
                        Close();
                        throw new Exception("No answer from device");
                    }
                    Thread.Sleep(2000);
                }
                else break;
            }
        }

        public void Dispose()
        {
            Close();
        }
    }
}
