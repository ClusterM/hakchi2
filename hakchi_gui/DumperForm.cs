using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class DumperForm : Form
    {
        public DumperForm()
        {
            InitializeComponent();
            hakchi.Initialize();
        }

        private void DumperForm_Load(object sender, EventArgs e)
        {
#if DUMPER
            if (MainForm.DoNand(Tasks.MembootTasks.NandTasks.DumpNand, Resources.DumpingNand, this))
            {
                Tasks.MessageForm.Show(Resources.Done, Resources.NandDumped, Resources.sign_check);
            }
#endif
            Close();
        }
    }
}
