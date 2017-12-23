using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Coralcode.Framework.Extensions;

namespace Coralcode.Framework.Reflection
{
    public class AssemblyFinder
    {
        private List<Assembly> _referencedAssemblies;
        public List<Assembly> GetAllAssemblies()
        {
            return ReferencedAssemblies;
        }

        public string GetAssemblySortName(Assembly assembly)
        {
            AssemblyName name = new AssemblyName(assembly.FullName);
            return name.Name;
        }


        /// <summary>
        /// 加载所有的程序集
        /// </summary>
        public  List<Assembly> ReferencedAssemblies
        {
            get
            {
                if (_referencedAssemblies != null) return _referencedAssemblies;
                Func<Assembly, bool> filter = assembly =>
                {
                    if (assembly == null)
                        return false;
                    if (assembly.FullName.StartsWith("mscorlib") ||
                        assembly.FullName.StartsWith("System") ||
                        assembly.FullName.StartsWith("Microsoft") ||
                        assembly.FullName.StartsWith("am.Charts") ||
                        assembly.FullName.StartsWith("ComponentArt") ||
                        assembly.FullName.StartsWith("Syncfusion") ||
                        assembly.FullName.StartsWith("LumiSoft") ||
                        assembly.FullName.StartsWith("Newtonsoft") ||
                        assembly.FullName.StartsWith("MySql") ||
                        assembly.FullName.StartsWith("Krystalware") ||
                        assembly.FullName.StartsWith("HtmlAgilityPack") ||
                        assembly.FullName.StartsWith("Ionic") ||
                        assembly.FullName.StartsWith("ICSharpCode") ||
                        assembly.FullName.StartsWith("eWebEditor") ||
                        assembly.FullName.StartsWith("App_Code") ||
                        assembly.FullName.StartsWith("App_global") ||
                        assembly.Location.StartsWith(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "obj")))
                        return false;
                    return true;
                };
                _referencedAssemblies = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.*", SearchOption.AllDirectories)
                    .Where(item => item.EndsWith(".dll") || item.EndsWith(".exe"))
                    .Select(item =>
                    {
                        try
                        {
                            return Assembly.LoadFrom(item);
                        }
                        catch (Exception)
                        {
                            return null;
                        }
                    })
                    .Where(filter)
                    .DistinctBy(item => item.FullName)
                    .ToList();
                return _referencedAssemblies;
            }
        }
    }
}
