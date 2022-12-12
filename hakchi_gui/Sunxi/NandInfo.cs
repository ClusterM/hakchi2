using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace com.clusterrr.hakchi_gui.Sunxi
{
    public class NandInfo
    {
        public class PartitionInfo
        {
            public readonly string Device;
            public readonly string Label;
            public readonly long Size;
            public readonly long BlockCount;


            internal PartitionInfo(string device, string label, long blocks)
            {
                this.Device = device;
                this.Label = label;
                this.Size = blocks * 512;
                this.BlockCount = blocks;
            }

            public static PartitionInfo GetByLabel(IEnumerable<PartitionInfo> info, params string[] labels)
            {
                return info.Where(e => labels.Contains(e.Label)).FirstOrDefault();
            }
        }

        public readonly long NandSize;
        public readonly long NandPageSize;
        public readonly long NandPagesPerBlock;
        public readonly long NandBlockCount;
        public readonly PartitionInfo[] Partitions;

        private NandInfo(PartitionInfo[] partitions, long pageSize, long pagesPerBlock, long blockCount)
        {
            this.NandSize = pageSize * pagesPerBlock * blockCount;
            this.NandPageSize = pageSize;
            this.NandPagesPerBlock = pagesPerBlock;
            this.NandBlockCount = blockCount;
            this.Partitions = partitions;
        }

        public static NandInfo GetNandInfo(string partCommand = "sunxi-part")
        {
            string partInfo = hakchi.Shell.ExecuteSimple(partCommand);
            string commandOutput = hakchi.Shell.ExecuteSimple("sunxi-flash nandinfo");

            if (string.IsNullOrWhiteSpace(commandOutput))
            {
                return new NandInfo(new PartitionInfo[] { }, 0, 0, 0);
            }

            string[] nandInfo = commandOutput.Split(' ');

            var partitions = new List<PartitionInfo>();

            foreach (Match item in Regex.Matches(partInfo, @"(mbr|nand[a-z]):(.*?):(\d+)"))
            {
                partitions.Add(new PartitionInfo(item.Groups[1].ToString(), item.Groups[2].ToString(), long.Parse(item.Groups[3].ToString())));
            }

            return new NandInfo(partitions.ToArray(), long.Parse(nandInfo[0]), long.Parse(nandInfo[1]), long.Parse(nandInfo[2]));
        }

        public PartitionInfo GetRootfsPartition() => PartitionInfo.GetByLabel(this.Partitions, "rootfs");
        public PartitionInfo GetDataPartition() => PartitionInfo.GetByLabel(this.Partitions, "data", "rootfs_data");
    }
}
