using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.clusterrr.ssh
{
    public interface IListener : IDisposable
    {
        IList<Device> Available
        {
            get;
        }
    }
}
