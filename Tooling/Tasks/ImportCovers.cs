using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.clusterrr.hakchi_gui.Tooling.Tasks
{

       class ImportCovers : TaskableTool
    {
        List<string> _ToImport;
        public ImportCovers(List<string> toImport) : base("Importing covers")
        {
            _ToImport = toImport;
        }
        public override void Execute()
        {
            ReportStatus("Importing...");
            for (int x = 0; x < _ToImport.Count; x++)
            {
                ReportStatus("Importing " + System.IO.Path.GetFileName(_ToImport[x]));
                Manager.CoverManager.getInstance().AddCover(_ToImport[x]);
                ReportProgress((x + 1) * 100 / _ToImport.Count);
            }
            ReportCompleted();
        }
    }
}
