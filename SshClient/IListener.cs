using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.clusterrr.ssh
{
    public interface IListener
    {
        IList<Device> Available
        {
            get;
        }

        void Abort();
    }
}
