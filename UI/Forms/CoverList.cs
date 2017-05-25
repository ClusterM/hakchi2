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
    public partial class CoverList : Form
    {
        public CoverList()
        {
            InitializeComponent();
            foreach (DataGridViewColumn column in this.dataGridView1.Columns)
                column.SortMode = DataGridViewColumnSortMode.Automatic;
            foreach (Manager.CoverManager.Cover r in Manager.CoverManager.getInstance().GetLibrary())
            {
                coverBindingSource.Add(r);
            }
        }
    }
}
