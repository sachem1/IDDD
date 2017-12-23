using System;
using System.Collections.Generic;
using System.Linq;

namespace Coralcode.Framework.Page
{
    public class PagedList<TDomain> : Page
        where TDomain : class
    {
        public PagedList(int total = 0, int size = 20, int current = 1, IList<TDomain> list = null)
        {
            Items = list ?? new List<TDomain>();
            TotalItemCount = total;
            TotalPageCount = (int)Math.Ceiling((double)total / size);
            PageSize = size;
            PageIndex = current;
        }

        public int TotalItemCount { get; set; }

        public int TotalPageCount { get; set; }

        public IList<TDomain> Items { get; protected set; }
    }


    public static class PageExtensions
    {
        public static PagedList<TDomain> ConvertToPagedList<TDomain>(this PagedList<TDomain> source)
            where TDomain : class
        {
            return new PagedList<TDomain>(source.TotalItemCount, source.PageSize, source.PageIndex, source.Items);
        }

        public static PagedList<TTarget> ConvertToPagedList<TDomain, TTarget>(this PagedList<TDomain> source, Func<TDomain, TTarget> mapFunc)
            where TDomain : class
            where TTarget : class
        {
            return new PagedList<TTarget>(source.TotalItemCount, source.PageSize, source.PageIndex, source.Items.Select(mapFunc).ToList());
        }

        public static DetailPagedList<TTarget> ConvertToDetailPagedList<TDomain, TTarget>(this PagedList<TDomain> source, List<TTarget> items)
            where TDomain : class
            where TTarget : class
        {
            return new DetailPagedList<TTarget>(source.TotalItemCount, source.PageSize, source.PageIndex, items);
        }
        public static DetailPagedList<TDomain> ConvertToDetailPagedList<TDomain>(this PagedList<TDomain> source)
            where TDomain : class
        {
            return new DetailPagedList<TDomain>(source.TotalItemCount, source.PageSize, source.PageIndex, source.Items);
        }

        public static DetailPagedList<TTarget> ConvertToDetailPagedList<TDomain, TTarget>(this PagedList<TDomain> source, Func<TDomain, TTarget> mapFunc)
            where TDomain : class
            where TTarget : class
        {
            return new DetailPagedList<TTarget>(source.TotalItemCount, source.PageSize, source.PageIndex, source.Items.Select(mapFunc).ToList());
        }
    }
}