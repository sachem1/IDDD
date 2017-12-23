using System;
using System.Collections.Generic;
using System.Linq;

namespace Coralcode.Framework.Page
{
    public class DetailPagedList<TDomain> : PagedList<TDomain>
        where TDomain : class
    {
        /// <summary>
        /// 两端显示页码与当前页的差值
        /// </summary>
        private const int ShowPaginationCount = 4;

        public DetailPagedList(int totelElementCount = 0, int size = 20, int currentPageIndex = 1, IList<TDomain> list = null)
        {

            PageIndex = currentPageIndex;
            TotalItemCount = totelElementCount;
            PageSize = size;
            TotalPageCount = (int)Math.Ceiling((double)totelElementCount / size);

            IsFirstPage = currentPageIndex == 1;
            IsLastPage = currentPageIndex == TotalPageCount;
            HasPreviousPage = !IsFirstPage;
            HasNextPage = !IsLastPage;

            PreviousPageIndex = IsFirstPage ? PageIndex : PageIndex - 1;

            NextPageIndex = IsLastPage ? PageIndex : PageIndex + 1;

            FirstItemIndex = (PageIndex - 1) * PageSize + 1;

            CurrentPageSize = TotalPageCount != currentPageIndex ? PageSize : TotalItemCount % PageSize;

            LastItemIndex = FirstItemIndex + CurrentPageSize - 1;

            InitShowPageinations();
            Items = list ?? new List<TDomain>();
        }

        public bool IsFirstPage { get; set; }

        public bool IsLastPage { get; set; }

        public bool HasNextPage { get; set; }

        public bool HasPreviousPage { get; set; }

        public int NextPageIndex { get; set; }

        public int PreviousPageIndex { get; set; }

        public int CurrentPageSize { get; set; }

        /// <summary>
        /// Begin with 0
        /// </summary>
        public int FirstItemIndex { get; set; }

        public int LastItemIndex { get; set; }

        public IEnumerable<int> PageArray { get; set; }

        public bool HasPage(int pageNumber)
        {
            return pageNumber > 0 && pageNumber <= TotalPageCount;
        }

        public TDomain FirstItem
        {
            get { return Items.FirstOrDefault(); }
        }

        public TDomain LastItem
        {
            get { return Items.LastOrDefault(); }
        }
        
        #region 初始化
        private void InitShowPageinations()
        {
            var tmpList = new List<int> { PageIndex };
            for (int i = 1; i <= ShowPaginationCount; i++)
            {
                int pagination;
                //往前数
                if (PageIndex - i < 1)
                {
                    //如果小于0，最后一个元素往后移动(i + ShowPaginationCount)个位置
                    pagination = PageIndex + (i + ShowPaginationCount);

                    if (pagination <= TotalPageCount)
                    {
                        tmpList.Add(pagination);
                    }
                }
                else
                {
                    pagination = PageIndex - i;
                    tmpList.Add(pagination);
                }
                //往后数
                if (PageIndex + i > TotalPageCount)
                {
                    //如果大于最大值，第一个元素，往前移动(i  + ShowPaginationCount)个位置
                    pagination = PageIndex - (i + ShowPaginationCount);
                    if (pagination > 0)
                    {
                        tmpList.Add(pagination);
                    }
                }
                else
                {
                    pagination = PageIndex + i;
                    tmpList.Add(pagination);
                }
            }

            PageArray = tmpList.OrderBy(item => item)
                                     .Select(item =>
                                     {
                                         if (item <= 0)
                                             return 1;
                                         return item >= TotalPageCount ? TotalPageCount : item;
                                     });
        }
        #endregion


    }
}