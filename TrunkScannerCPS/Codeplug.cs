using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrunkScannerCPS
{
    public class Codeplug
    {
        public List<Zone> Zones { get; set; } = new List<Zone>();
        public List<ScanList> ScanLists { get; set; } = new List<ScanList>();
        public string ModelNumber { get; set; }
        public string SerialNumber { get; set; }
        public string CodeplugVersion {  get; set; }
        public int LastProgramSource { get; set; }
        public string FlickerCode { get; set; }
        public bool RadioKilled { get; set; }
        public bool TrunkingInhibited { get; set; }
        public bool TtsEnabled { get; set; }

        public bool IsValid()
        {
            bool result = true;

            if (SerialNumber.Length != 10)
                result = false;

            if (ModelNumber.Length != 7)
                result = false;

            if (Zones == null)
                result = false;

            if (LastProgramSource > 3)
                result = false;

            if (CodeplugVersion == null)
                result = false;

            return result;
        }

        public bool IsKilled()
        {
            return RadioKilled;
        }

        public bool IsTrunkingInhibited()
        {
            return TrunkingInhibited;
        }
    }

    public class Zone
    {
        public string Name { get; set; }
        public string ScanListName { get; set; }

        public List<Channel> Channels { get; set; } = new List<Channel>();
    }

    public class Channel
    {
        public string Alias { get; set; }
        public string Tgid { get; set; }
    }

    public class ScanList
    {
        public string Name { get; set; }
        public List<ScanListItem> Items { get; set; } = new List<ScanListItem>();
    }

    public class ScanListItem
    {
        public string Alias { get; set; }
        public string Tgid { get; set; }
    }

    public enum CodeplugSource
    {
        CPS,
        Depot,
        Labtool,
        PhpSplutions
    }
}
