using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui.UI.Forms
{
    public partial class RomsList : Form
    {
        public RomsList()
        {
            InitializeComponent();
            foreach (DataGridViewColumn column in this.dataGridView1.Columns)
                column.SortMode = DataGridViewColumnSortMode.Automatic;
            foreach (Manager.RomManager.Rom r in Manager.RomManager.getInstance().GetLibrary())
            {
                romBindingSource.Add(r);
            }
        }
    }
}
