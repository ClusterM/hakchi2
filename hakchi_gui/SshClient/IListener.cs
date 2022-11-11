using System;
using System.Collections.Generic;

namespace com.clusterrr.ssh
{
    public interface IListener : IDisposable
    {
        IList<Device> Available
        {
            get;
        }

        void Cycle();
    }
}
