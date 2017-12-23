using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Coralcode.Framework.Extensions;
using Coralcode.Framework.Log;

namespace Coralcode.Framework.Reflection
{
    public class TypeFinder
    {
        private List<Type> _allTypes;


        public IEnumerable<Type> GetCurrentAssemblyTypes()
        {
            return Assembly.GetCallingAssembly().GetTypes();
        }

        public IEnumerable<Type> Find(Func<Type, bool> predicate)
        {
            return GetAll().Where(predicate);
        }

        public IEnumerable<Type> GetAll()
        {
            if (_allTypes != null)
                return _allTypes;
            _allTypes = new List<Type>();
            foreach (var assembly in MetaDataManager.Assembly.ReferencedAssemblies)
            {
                try
                {
                    Type[] typesInThisAssembly;
                    try
                    {
                        typesInThisAssembly = assembly.GetTypes();
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        typesInThisAssembly = ex.Types;
                    }

                    if (typesInThisAssembly == null || typesInThisAssembly.Length == 0)
                    {
                        continue;
                    }
                    _allTypes.AddRange(typesInThisAssembly.Where(type => type != null));
                }
                catch (Exception ex)
                {
                    LoggerFactory.Instance.Warning(ex.ToString(), ex);
                }
            }
            return _allTypes;
        }


    }
}
