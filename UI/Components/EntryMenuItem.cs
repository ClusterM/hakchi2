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
        Manager.BookManager.Book _TheBook;
        Manager.BookManager.Page _ThePage;
        public EntryMenuItem(Manager.BookManager.Entry entr,Manager.BookManager.Page page,Manager.BookManager.Book book):base(entr.Label)
        {
            _TheEntry = entr;
            _TheBook = book;
            _ThePage = page;
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
            UI.EntryCreator ec = new EntryCreator(_TheBook);
            ec.EditEntry(_TheEntry);
            ec.ShowDialog();

        }
    }
}
