using com.clusterrr.hakchi_gui.Hmod.Controls;
using com.clusterrr.hakchi_gui.ModHub.Controls;
using com.clusterrr.hakchi_gui.Properties;
using System;
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
        public void LoadData(Repository.Repository repo)
        {
            tabControl1.SuspendLayout();

            if(repo.Readme != null && repo.Readme.Trim() != "")
            {
                var page = new TabPage(Resources.Welcome);
                var readmeControl = new ReadmeControl();

                readmeControl.Dock = DockStyle.Fill;
                readmeControl.setReadme(null, repo.Readme, true);

                page.Controls.Add(readmeControl);
                tabControl1.TabPages.Add(page);
            }

            var categories = new List<string>();
            IEnumerable<Repository.Repository.Item> items;
            foreach (var mod in repo.Items)
            {
                if (mod.Category != null && categories.FindIndex(o => o.Equals(mod.Category, StringComparison.OrdinalIgnoreCase)) == -1)
                {
                    categories.Add(mod.Category);
                }
            }

            categories.Sort();

            categories.Add(null);

            foreach (var category in categories)
            {
                items = repo.Items.Where((o) => o.Kind != Repository.Repository.ItemKind.Game && ((o.Category != null && o.Category.Equals(category, System.StringComparison.OrdinalIgnoreCase)) || o.Category == null && category == null));
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

            items = repo.Items.Where((o) => o.Kind == Repository.Repository.ItemKind.Game);

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
            
            tabControl1.ResumeLayout();
        }
    }
}
