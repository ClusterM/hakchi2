using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.clusterrr.hakchi_gui.Tooling.Tasks
{
    public class SaveCurrentConfigAsBook : TaskableTool
    {
        public SaveCurrentConfigAsBook() : base("Saving current config as book")
        {
        }
        public override void Execute()
        {
            ReportStatus("Importing all selected games in library");
            IOrderedEnumerable<NesMiniApplication> selectedGames = Manager.GameManager.GetInstance().getSelectedGames();
            int processed = 0;
            foreach(NesMiniApplication mn in selectedGames)
            {
                string localRom = System.IO.Path.Combine(mn.GamePath, mn.RomFile);
                if(System.IO.File.Exists(localRom))
                {
                    Manager.RomManager.getInstance().AddRom(localRom);

                }
                Manager.CoverManager.getInstance().AddCover(mn.IconPath);
                processed++;
                ReportProgress(processed * 100 / selectedGames.Count());
                Console.Write(mn.RomFile);
            }
            NesMenuCollection nmc = new NesMenuCollection();
            ReportCompleted();
        }
    }
}
