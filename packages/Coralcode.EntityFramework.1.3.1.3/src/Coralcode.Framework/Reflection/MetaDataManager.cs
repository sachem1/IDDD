namespace Coralcode.Framework.Reflection
{
    /// <summary>
    /// 元数据管理
    /// </summary>
    public class MetaDataManager
    {
        /// <summary>
        /// 获取程序集
        /// </summary>
        public static readonly AssemblyFinder Assembly = new AssemblyFinder();

        /// <summary>
        /// 获取类型
        /// </summary>
        public static readonly TypeFinder Type = new TypeFinder();

        /// <summary>
        /// 获取特性
        /// </summary>
        public static readonly AttributeFinder Attribute = new AttributeFinder();
    }
}
