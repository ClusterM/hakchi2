using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui.Hmod.Controls
{
    public partial class ReadmeControl : UserControl, IReadmeControl
    {
        IReadmeControl readmeControl;
        public ReadmeControl()
        {
            InitializeComponent();
            if (Shared.isWindows)
            {
                readmeControl = new BrowserReadmeControl();
            }
            else
            {
                readmeControl = new TextReadmeControl();
            }
            ((ContainerControl)readmeControl).Dock = DockStyle.Fill;
            Controls.Add((ContainerControl)readmeControl);
        }

        public void clear()
        {
            readmeControl.clear();
        }

        public void setReadme(string name, string readme, bool markdown = false)
        {
            readmeControl.setReadme(name, readme, markdown);
        }

        public void setReadme(string name, HmodReadme hReadme)
        {
            readmeControl.setReadme(name, hReadme);
        }
    }
}
