using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui.UI
{
    public class FixedTreeview :TreeView
    {
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x203) // identified double click
            {
                var localPos = PointToClient(Cursor.Position);
                var hitTestInfo = HitTest(localPos);
                if (hitTestInfo.Location == TreeViewHitTestLocations.StateImage)
                    m.Result = IntPtr.Zero;
                else
                    base.WndProc(ref m);
            }
            else base.WndProc(ref m);
        }
    }
}
