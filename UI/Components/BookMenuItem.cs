using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace com.clusterrr.hakchi_gui.UI.Components
{
    class BookMenuItem:ToolStripMenuItem
    {
        Manager.BookManager.Book _TheBook;
        public BookMenuItem(Manager.BookManager.Book b):base(b.Name)
        {
            _TheBook = b;
            this.DropDownOpening += BookMenuItem_DropDownOpening;
            this.DropDownItems.Add(new ToolStripSeparator());
        }
        ToolStripTextBox pageName = new ToolStripTextBox();
        private void BookMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            DropDownItems.Clear();

            foreach (Manager.BookManager.Page ee in _TheBook.Pages)
            {
                DropDownItems.Add(new PageMenuItem(ee,_TheBook));
            }
            this.DropDownItems.Add(new ToolStripSeparator());
            ToolStripMenuItem ts = new ToolStripMenuItem("Add Page");
            ts.Click += Ts_Click;
            
      
            ts.DropDownItems.Add(pageName);

            ToolStripMenuItem doAdd = new ToolStripMenuItem("Add!");
            doAdd.Click += DoAdd_Click;

            ts.DropDownItems.Add(doAdd);

            DropDownItems.Add(ts);
            ToolStripMenuItem dts = new ToolStripMenuItem("Delete Book");
            dts.Click += Dts_Click;
            DropDownItems.Add(dts);
        }

        private void DoAdd_Click(object sender, EventArgs e)
        {
            
            if (pageName.Text.Trim() != "")
            {
                
                _TheBook.AddPage(pageName.Text);
            }
        }

        private void Dts_Click(object sender, EventArgs e)
        {

        }

        private void Ts_Click(object sender, EventArgs e)
        {

           
        }
    }
}
