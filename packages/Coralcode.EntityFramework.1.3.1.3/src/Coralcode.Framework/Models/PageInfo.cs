namespace Coralcode.Framework.Models
{
    public class PageInfo
    {
        public PageInfo()
        {
            PageIndex = 1;
            PageSize = 10;
        }

        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get; set; }


        /// <summary>
        /// 跳过的条数，固定
        /// </summary>
        public int SkipCount
        {
            get { return (PageIndex - 1) * PageSize; }
        }
    }
}