using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace com.clusterrr.hakchi_gui.UI.Components
{
    public class EntryMenuItem : System.Windows.Forms.ToolStripMenuItem
    {
        Manager.BookManager.Entry _TheEntry;
        public EntryMenuItem(Manager.BookManager.Entry entr):base(entr.Label)
        {
            _TheEntry = entr;
            ToolStripMenuItem entryEdititm = new ToolStripMenuItem("Edit");
            entryEdititm.Click += EntryEdititm_Click;
          
            this.DropDownItems.Add(entryEdititm);
            ToolStripMenuItem entryDeleteitm = new ToolStripMenuItem("Delete");
            entryDeleteitm.Click += EntryDeleteitm_Click; ; ;

            this.DropDownItems.Add(entryDeleteitm);
        }

        private void EntryDeleteitm_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void EntryEdititm_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
