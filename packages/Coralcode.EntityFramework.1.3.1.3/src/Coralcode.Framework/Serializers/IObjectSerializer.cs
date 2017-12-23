using System.IO;

namespace Coralcode.Framework.Serializers
{
    public interface IObjectSerializer<T> where T : class 
    {
        /// <summary>
        /// get entity from stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        T Load(Stream stream);

        /// <summary>
        /// save stream in entity  
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="entity"> </param>
        void Save(Stream stream,T entity);


        /// <summary>
        /// get entity from file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        T LoadFromFile(string filePath);

        /// <summary>
        /// save entity in file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="entitiy"></param>
        void SaveInFile(string filePath, T entitiy);

        /// <summary>
        /// Serialize entity to memorystream
        /// </summary>
        /// <param name="entntiy"></param>
        /// <returns></returns>
        MemoryStream SerializeToMemory(T entntiy);

        /// <summary>
        /// get entity from memorystream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        T DeserlizeFromMemery(MemoryStream stream);
    }
}
