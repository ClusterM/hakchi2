using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui.Hmod.Controls
{
    interface IReadmeControl : IContainerControl
    {
        void setReadme(string name, string readme, bool markdown);
        void setReadme(string name, HmodReadme hReadme);
        void clear();
    }
}
