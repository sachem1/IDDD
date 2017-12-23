using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Reflection
{
    public class AssemblyFinder
    {
        private List<Assembly> _reflenceAssemblyList;


        public List<Assembly> LoadAllReferenceAssembly()
        {
            if (_reflenceAssemblyList == null)
                _reflenceAssemblyList = new List<Assembly>();

            Func<Assembly, bool> filter = assembly => {
                if (assembly.FullName.StartsWith("mscorlib") ||
                assembly.FullName.StartsWith("System"))
                    return false;
                return true;
            };
            _reflenceAssemblyList= Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.*", SearchOption.AllDirectories).Where(k => k.EndsWith(".dll") || k.EndsWith(".exe"))
                .Select(item =>
                {
                    try
                    {
                        return Assembly.Load(item);
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }).Where(filter).Distinct().ToList();


            return _reflenceAssemblyList;
        }
    }
}
