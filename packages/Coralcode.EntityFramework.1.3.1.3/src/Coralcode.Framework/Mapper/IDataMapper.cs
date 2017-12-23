

namespace Coralcode.Framework.Mapper
{
    public interface IDataMapper
    {
        TTarget Convert<TSource, TTarget>(TSource source)
            where TTarget : class, new()
            where TSource : class;

        TTarget Convert<TSource, TTarget>(TSource source, TTarget target)
            where TTarget : class
            where TSource : class;

        /// <summary>
        /// 带配置文件的转换
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="config">配置文件</param>
        /// <returns></returns>
        TTarget Convert<TSource, TTarget>(TSource source, TTarget target, dynamic config)
            where TTarget : class
            where TSource : class;

        /// <summary>
        /// 带配置文件的转换
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="source"></param>
        /// <param name="config">配置文件</param>
        /// <returns></returns>
        TTarget Convert<TSource, TTarget>(TSource source, dynamic config)
            where TTarget : class, new()
            where TSource : class;
    }
}