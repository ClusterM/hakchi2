using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace com.clusterrr.hakchi_gui.UI.Components
{
    class PageMenuItem : ToolStripMenuItem
    {
        private Manager.BookManager.Book _theBook;
        private Manager.BookManager.Page _ThePage;
        public PageMenuItem(Manager.BookManager.Page thePage, Manager.BookManager.Book theBook) : base(thePage.FriendlyName)
        {
            _ThePage = thePage;
            _theBook = theBook;
            this.DropDownOpening += PageMenuItem_DropDownOpening;
            this.DropDownItems.Add(new ToolStripSeparator());
        }

        private void PageMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            DropDownItems.Clear();

            foreach (Manager.BookManager.Entry ee in _ThePage.Entries)
            {
                DropDownItems.Add(new EntryMenuItem(ee,_ThePage,_theBook));
            }
            this.DropDownItems.Add(new ToolStripSeparator());
            ToolStripMenuItem ts = new ToolStripMenuItem("Add Entry");
            ts.Click += Ts_Click;
            DropDownItems.Add(ts);
            ToolStripMenuItem dts = new ToolStripMenuItem("Delete page");
            dts.Click += Dts_Click;
            DropDownItems.Add(dts);
        }

        private void Dts_Click(object sender, EventArgs e)
        {
           
        }

        private void Ts_Click(object sender, EventArgs e)
        {

            UI.EntryCreator ec = new UI.EntryCreator(_theBook);

            if (ec.ShowDialog() == DialogResult.OK)
            {
                _ThePage.Entries.Add(ec.entr);
                Manager.BookManager.getInstance().SaveSettings();
            }
            Console.Write("");
        }
    }
}
