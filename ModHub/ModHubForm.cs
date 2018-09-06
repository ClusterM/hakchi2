using com.clusterrr.hakchi_gui.ModHub.Controls;
using com.clusterrr.hakchi_gui.Properties;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui.ModHub
{
    public partial class ModHubForm : Form
    {
        List<Hmod.Hmod> installedMods;
        public ModHubForm()
        {
            InitializeComponent();
            installedMods = Hmod.Hmod.GetMods();
        }
        public void LoadData(IEnumerable<Repository.Repository.Item> repoItems)
        {
            var categories = new List<string>();
            IEnumerable<Repository.Repository.Item> items;
            foreach (var mod in repoItems)
            {
                if (mod.Category != null && !categories.Contains(mod.Category))
                {
                    categories.Add(mod.Category);
                }
            }

            categories.Sort();

            categories.Add(null);

            foreach (var category in categories)
            {
                items = repoItems.Where((o) => o.Kind != Repository.Repository.ItemKind.Game && ((o.Category != null && o.Category.Equals(category, System.StringComparison.CurrentCultureIgnoreCase)) || o.Category == null && category == null));
                if (items.Count() == 0)
                    continue;

                var page = new TabPage(category ?? Resources.Unknown);
                var tabControl = new ModHubTabControl();
                tabControl.SetInstallButtonState(true);
                tabControl.parentForm = this;
                tabControl.installedMods = installedMods;
                tabControl.LoadData(items);
                tabControl.Dock = DockStyle.Fill;
                page.Controls.Add(tabControl);
                tabControl1.TabPages.Add(page);
            }

            items = repoItems.Where((o) => o.Kind == Repository.Repository.ItemKind.Game);

            if(items.Count() > 0)
            {
                var page = new TabPage("Games");
                var tabControl = new ModHubTabControl();
                tabControl.SetInstallButtonState(true);
                tabControl.parentForm = this;
                tabControl.installedMods = installedMods;
                tabControl.LoadData(items);
                tabControl.GroupList("Category");
                tabControl.SetInstallButtonState(false);
                tabControl.SetButtonStrings(ModHubTabControl.ButtonStringType.Game);
                tabControl.Dock = DockStyle.Fill;
                page.Controls.Add(tabControl);
                tabControl1.TabPages.Add(page);
            }

        }
    }
}
