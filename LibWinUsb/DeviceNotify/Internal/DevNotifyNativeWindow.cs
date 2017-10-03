// Copyright © 2006-2010 Travis Robinson. All rights reserved.
// 
// website: http://sourceforge.net/projects/libusbdotnet
// e-mail:  libusbdotnet@gmail.com
// 
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the
// Free Software Foundation; either version 2 of the License, or 
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License
// for more details.
// 
// You should have received a copy of the GNU General Public License along
// with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA. or 
// visit www.gnu.org.
// 
// 
using System;
using System.Windows.Forms;

namespace LibUsbDotNet.DeviceNotify.Internal
{
    internal sealed class DevNotifyNativeWindow : NativeWindow
    {
        private const string WINDOW_CAPTION = "{18662f14-0871-455c-bf99-eff135425e3a}";
        private const int WM_DEVICECHANGE = 0x219;
        private readonly OnDeviceChangeDelegate mDelDeviceChange;
        private readonly OnHandleChangeDelegate mDelHandleChanged;

        internal DevNotifyNativeWindow(OnHandleChangeDelegate delHandleChanged, OnDeviceChangeDelegate delDeviceChange)
        {
            mDelHandleChanged = delHandleChanged;
            mDelDeviceChange = delDeviceChange;

            CreateParams cp = new CreateParams();
            cp.Caption = WINDOW_CAPTION;
            cp.X = -100;
            cp.Y = -100;
            cp.Width = 50;
            cp.Height = 50;
            CreateHandle(cp);
        }

        protected override void OnHandleChange()
        {
            mDelHandleChanged(Handle);
            base.OnHandleChange();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_DEVICECHANGE)
            {
                mDelDeviceChange(ref m);
            }
            base.WndProc(ref m);
        }

        #region Nested Types

        #region Nested type: OnDeviceChangeDelegate

        internal delegate void OnDeviceChangeDelegate(ref Message m);

        #endregion

        #region Nested type: OnHandleChangeDelegate

        internal delegate void OnHandleChangeDelegate(IntPtr windowHandle);

        #endregion

        #endregion
    }
}