using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.clusterrr.hakchi_gui.Tooling.Tasks
{
    class ImportRoms:TaskableTool
    {
        List<string> _ToImport;
        public ImportRoms(List<string> toImport):base("Import roms")
        {
            _ToImport = toImport;
        }
        public override void Execute()
        {
            ReportStatus("Importing...");
            for (int x = 0; x < _ToImport.Count; x++)
            {
                ReportStatus("Importing " + System.IO.Path.GetFileName(_ToImport[x]));
                Manager.RomManager.getInstance().AddRom(_ToImport[x]);
                ReportProgress((x + 1) * 100 / _ToImport.Count);
            }
            ReportCompleted();
        }
    }
}
