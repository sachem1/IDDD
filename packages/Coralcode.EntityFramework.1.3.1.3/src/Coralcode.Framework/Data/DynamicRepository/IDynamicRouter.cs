namespace Coralcode.Framework.Data.DynamicRepository
{
    /// <summary>
    /// 分库的策略
    /// </summary>
    public interface IDynamicRouter
    {
        /// <summary>
        /// 分库的策略因子
        /// </summary>
        string Coden { get; }
    }

    public class SampleRouter : IDynamicRouter
    {
        public SampleRouter(string coden)
        {
            Coden = coden;
        }

        /// <summary>
        /// 分库的策略因子
        /// </summary>
        public string Coden { get; }
    }
}
