using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLP_Constructor.Model.Supported
{
    public static class ProviderSelector
    {
        private static readonly Dictionary<string, string[]> extensions = new Dictionary<string, string[]>();
        private static readonly Dictionary<string, string> providers = new Dictionary<string, string>();
        static ProviderSelector()
        {
            extensions.Add("Access files",
                new[] { ".accdb", ".mdb" });
            providers.Add("Access files", "Microsoft.ACE.OLEDB.12.0");
        }
        public static string GetAvaliableFilter()
        {
            List<string> parts = new List<string>();
            foreach (var dbType in extensions.Select(x => x.Key))
            {
                parts.Add(dbType);
                parts.Add(string.Join(";", extensions[dbType].Select(x => x.Replace(".", "*."))));
            }
            return string.Join("|", parts);
        }
        public static bool TrySelect(string fileExtension, out string provider)
        {
            var fileGroupName = extensions
                .Where(x => x.Value.Contains(fileExtension))
                .Select(x => x.Key)
                .FirstOrDefault();
            if(fileGroupName is null || !providers.ContainsKey(fileGroupName))
            {
                provider = null;
                return false;
            }
            
            provider = providers[fileGroupName];
            return true;
        }
    }
}
