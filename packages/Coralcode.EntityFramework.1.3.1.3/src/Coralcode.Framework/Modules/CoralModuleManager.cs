using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Coralcode.Framework.Aspect;
using Coralcode.Framework.Aspect.Unity;
using Coralcode.Framework.Exceptions;
using Coralcode.Framework.Extensions;
using Coralcode.Framework.Reflection;
using Coralcode.Framework.Arithmetic;

namespace Coralcode.Framework.Modules
{
    [Inject(RegisterType = typeof(IModuleManager), LifetimeManagerType = LifetimeManagerType.ContainerControlled)]
    public class CoralModuleManager : IModuleManager
    {
        private Dictionary<string, CoralModule> _coralModules;
        private Dictionary<Type, CoralModule> _typeModules;
        private IList<CoralModule> _modules;


        public CoralModule GetModule(Func<CoralModule, bool> predicate)
        {
            return _modules.FirstOrDefault(predicate);
        }

        public CoralModule GetModule(Type type)
        {
            if (!_typeModules.ContainsKey(type))
                return null;
            return _typeModules[type];
        }

        public CoralModule GetModule(string moduleName)
        {
            if (!_coralModules.ContainsKey(moduleName))
                return null;
            return _coralModules[moduleName];
        }

        public IEnumerable<CoralModule> GetModules(Func<CoralModule, bool> predicate)
        {
            return _modules.Where(predicate);
        }

        public void InstallModules()
        {
            try
            {
                _modules = MetaDataManager.Type.Find(CoralModule.IsCoralModule)
                            .SortByDependencies(item =>
                            {
                                //所有的模块都默认依赖于CoralModule
                                if (item == typeof(CoreModule))
                                    return new List<Type>();
                                var dependencies = DependencyAttribute.GetDependencies(item).Where(type => !type.IsAbstract).ToList();
                                dependencies.Add(typeof(CoreModule));
                                return dependencies;
                            })
                            .Select(item => UnityService.Resolve(item) as CoralModule)
                            .Where(item => item != null)
                            .ToList();
                _coralModules = new Dictionary<string, CoralModule>();
                _typeModules = new Dictionary<Type, CoralModule>();

                //构建名字和模块的字典
                foreach (var module in _modules)
                {
                    if (_coralModules.ContainsKey(module.Name))
                        throw CoralException.ThrowException(item => item.ModuleExisted, module.Name);
                    _coralModules.Add(module.Name, module);
                }
                //构建类型
                foreach (var module in _modules)
                {
                    foreach (var type in module.Types)
                    {
                        if (!_typeModules.ContainsKey(type))
                            _typeModules.Add(type, module);
                    }
                }
                _modules.ForEach(item => item.Prepare());
                _modules.ForEach(item => item.Install());
                _modules.ForEach(item => item.Installed());
            }
            catch (ReflectionTypeLoadException ex)
            {
                throw new CoralModuleException(ex);
            }
        }

        public void UninstallModules()
        {
            _modules.ForEach(item => item.UnInstall());
        }

    }
}
