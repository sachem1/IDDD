using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Algorithm
{
    public class GenericsFactory<T> : IGenericsFactory<T> where T : IAlgorithmStrategy
    {
        protected Dictionary<string, Type> Strategys;
        protected Func<Type, T> Creator;
        public void InitStrategy()
        {
            if (Strategys == null) {

            }
        }
        public void CreatorSetting(Func<Type, T> creator)
        {
            throw new NotImplementedException();
        }

        public T GetStrategy(string name)
        {
            throw new NotImplementedException();
        }

        public List<T> GetStrategyList()
        {
            throw new NotImplementedException();
        }

        public Type GetStrategyType()
        {
            throw new NotImplementedException();
        }

        public void Register(Type type)
        {
            throw new NotImplementedException();
        }
    }

    public static class CommonHelp
    {


        /// <summary>
        /// 去重
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="Tkey"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(selector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}
