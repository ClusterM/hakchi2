using System.Collections.Generic;

namespace nQuant
{
    public class LookupData
    {
        public LookupData(int granularity)
        {
            Lookups = new List<Lookup>();
            Tags = new int[granularity, granularity, granularity, granularity];
        }

        public IList<Lookup> Lookups { get; private set; }
        public int[, , ,] Tags { get; private set; }
    }
}