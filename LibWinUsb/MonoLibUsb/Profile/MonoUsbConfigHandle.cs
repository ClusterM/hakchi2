using System;
using System.Runtime.InteropServices;
using LibUsbDotNet.Main;
using MonoLibUsb.Descriptors;

namespace MonoLibUsb.Profile
{
    /// <summary>
    /// The <see cref="MonoUsbConfigHandle"/> class hold the internal pointer to a libusb <see cref="MonoUsbConfigDescriptor"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// To acquire a <see cref="MonoUsbConfigHandle"/> use:
    /// <list type="bullet">
    /// <item><see cref="MonoUsbApi.GetActiveConfigDescriptor"/></item>
    /// <item><see cref="MonoUsbApi.GetConfigDescriptor"/></item>
    /// <item><see cref="MonoUsbApi.GetConfigDescriptorByValue"/></item>
    /// </list>
    /// </para>
    /// <para>To access configuration information see <see cref="MonoUsbConfigDescriptor(MonoUsbConfigHandle)"/>.</para>
    /// <example><code source="..\MonoLibUsb\MonoUsb.ShowConfig\ShowConfig.cs" lang="cs"/></example>
    /// </remarks>
    public class MonoUsbConfigHandle:SafeContextHandle
    {
        private MonoUsbConfigHandle() : base(IntPtr.Zero,true) {}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool ReleaseHandle() 
        {
            if (!IsInvalid)
            {
                MonoUsbApi.FreeConfigDescriptor(handle);
                SetHandleAsInvalid();
            }
            return true;
        }
    }
}
