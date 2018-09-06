using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.clusterrr.hakchi_gui.ModHub.Repository
{
    public struct RepositoryInfo
    {
        public string Name { get; set; }
        public string URL { get; set; }
        public RepositoryInfo(string name, string url)
        {
            Name = name;
            URL = url;
        }
    }
}
