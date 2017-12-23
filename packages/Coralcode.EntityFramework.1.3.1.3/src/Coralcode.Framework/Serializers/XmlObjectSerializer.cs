using System;
using System.IO;
using System.Xml.Serialization;

namespace Coralcode.Framework.Serializers
{
    public class XmlObjectSerializer<T> : IObjectSerializer<T>
        where T : class
    {
        /// <summary>
        /// get entity from stream
        /// Exception :
        ///     ArgumentNullException
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public T Load(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            var xml = new XmlSerializer(typeof(T));

            return xml.Deserialize(stream) as T;

        }

        /// <summary>
        /// save stream in entity  
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="entity"> </param>
        public void Save(Stream stream, T entity)
        {
            var xml = new XmlSerializer(typeof(T));
            xml.Serialize(stream, entity);
        }

        /// <summary>
        /// get entity from file
        /// Exception :
        ///     FileNotFoundException
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public T LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException(filePath);

            using(var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                return Load(stream);
            }
        }

        /// <summary>
        /// save entity in file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="entitiy"></param>
        public void SaveInFile(string filePath, T entitiy)
        {
            using(var stream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite))
            {
                Save(stream, entitiy);
            }
        }

        /// <summary>
        /// Serialize entity to memorystream
        /// </summary>
        /// <param name="entntiy"></param>
        /// <returns></returns>
        public MemoryStream SerializeToMemory(T entntiy)
        {
            using (var stream = new MemoryStream()) {
                Save(stream, entntiy);
                return stream;
            }
        }

        /// <summary>
        /// get entity from memorystream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public T DeserlizeFromMemery(MemoryStream stream)
        {
            return Load(stream);
        }
    }
}
