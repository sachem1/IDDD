using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Algorithm
{
    public interface IGenericsFactory<T>where T:IAlgorithmStrategy
    {
        /// <summary>
        /// 可以通过外部配置
        /// </summary>
        /// <param name="creator"></param>
        void CreatorSetting(Func<Type, T> creator);

        void Register(Type type);
        /// <summary>
        /// 根据名称获取
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        T GetStrategy(string name);

        List<T> GetStrategyList();

        Type GetStrategyType();
    }
}
