using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class SelectCoreDialog : Form
    {
        Dictionary<string, NesApplication> gameList;
        Dictionary<string, NesApplication.AppMetadata> gameSet;

        private void FillSystems()
        {
            listBoxSystem.Items.Clear();
            listBoxSystem.Items.Add("Unassigned");
            foreach(var system in CoreCollection.Systems)
            {
                listBoxSystem.Items.Add(system);
            }
        }

        private void FillCores(string system)
        {
            listBoxCore.Items.Clear();
            var cores = CoreCollection.GetCoresFromSystem(system);
            if (cores == null)
                return;
            foreach (var core in cores)
            {
                listBoxCore.Items.Add(core.Name);
            }
        }

        public SelectCoreDialog()
        {
            InitializeComponent();
            FillSystems();
            DialogResult = DialogResult.Abort;
        }

        private void buttonDiscard_Click(object sender, EventArgs e)
        {
            if( MessageBox.Show("Are you sure you want to assign all games to unknown application?", "Discard changes", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                Close();
            }
        }

        private void listBoxSystem_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = (string)listBoxSystem.SelectedItem;
            FillCores(item);
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void listBoxCore_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
