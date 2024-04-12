using System;
using System.Collections.Generic;
using System.Linq;

namespace TrunkScannerCPS
{
    internal class AppType
    {
        public string Name { get; private set; }
        public bool AreFieldsEditable { get; private set; }

        public static readonly AppType CPS = new AppType("CPS", true);
        public static readonly AppType Depot = new AppType("Depot", false);
        public static readonly AppType Labtool = new AppType("Labtool", true);
        public static readonly AppType PhpSolutions = new AppType("PhpSplutions", false);

        private AppType(string name, bool areFieldsEditable)
        {
            Name = name;
            AreFieldsEditable = areFieldsEditable;
        }

        public static IEnumerable<AppType> GetAll()
        {
            return new[] { CPS, Depot, Labtool, PhpSolutions };
        }

        public override string ToString()
        {
            return Name;
        }

        public static AppType FromName(string name)
        {
            return GetAll().FirstOrDefault(appType => appType.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}