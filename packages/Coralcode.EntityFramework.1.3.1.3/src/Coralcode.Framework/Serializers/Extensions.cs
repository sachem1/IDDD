using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coralcode.Framework.Mapper;

namespace Coralcode.Framework.Serializers
{
    public static class Extensions
    {
        public static TEntity LoadFormXml<TEntity>(this TEntity entity, string file) where TEntity : class 
        {
            using (Stream stream = File.OpenRead(file))
            {
                return entity.LoadFormXml(stream);
            }
        }

        public static TEntity LoadFormXml<TEntity>(this TEntity entity, Stream stream) where TEntity:class
        {
            var tmp = new XmlObjectSerializer<TEntity>().Load(stream);
            DataMapperProvider.Mapper.Convert(tmp, entity);
            return tmp;
        }

        public static void SaveToXml<TEntity>(this TEntity entity, string file) where TEntity : class
        {
            var path = Path.GetDirectoryName(file);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var stream = File.Create(file);

            entity.SaveToXml(stream);
            stream.Close();
        }

        public static void SaveToXml<TEntity>(this TEntity entity, Stream stream) where TEntity : class
        {
            new XmlObjectSerializer<TEntity>().Save(stream, entity);
        }
    }
}
