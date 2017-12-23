using EmitMapper;
using EmitMapper.MappingConfiguration;

namespace Coralcode.Framework.Mapper
{
    public class EmitmapperDataMapper
        : IDataMapper
    {
        #region ITypeAdapter Members

        public TTarget Convert<TSource, TTarget>(TSource source)
            where TSource : class
            where TTarget : class, new()
        {
            ObjectsMapper<TSource, TTarget> mapper =
                ObjectMapperManager.DefaultInstance.GetMapper<TSource, TTarget>(new DefaultMapConfig());
            return mapper.Map(source);
        }

        public TTarget Convert<TSource, TTarget>(TSource source, TTarget target)
            where TTarget : class
            where TSource : class
        {
            ObjectsMapper<TSource, TTarget> mapper =
                ObjectMapperManager.DefaultInstance.GetMapper<TSource, TTarget>(new DefaultMapConfig());
            return mapper.Map(source, target);
        }


        public TTarget Convert<TSource, TTarget>(TSource source, TTarget target, dynamic config)
            where TTarget : class
            where TSource : class
        {
            ObjectsMapper<TSource, TTarget> mapper =
                ObjectMapperManager.DefaultInstance.GetMapper<TSource, TTarget>((DefaultMapConfig)config);
            return mapper.Map(source, target);
        }


        public TTarget Convert<TSource, TTarget>(TSource source, dynamic config)
            where TTarget : class, new()
            where TSource : class
        {
            ObjectsMapper<TSource, TTarget> mapper =
                ObjectMapperManager.DefaultInstance.GetMapper<TSource, TTarget>(config);
            return mapper.Map(source);
        }

        #endregion
    }
}