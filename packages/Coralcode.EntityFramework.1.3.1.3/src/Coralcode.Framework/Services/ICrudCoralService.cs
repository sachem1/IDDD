using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coralcode.Framework.Models;
using Coralcode.Framework.Page;

namespace Coralcode.Framework.Services
{
    /// <summary>
    /// 全功能增删改查服务
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TSearch"></typeparam>
    /// <typeparam name="TOrder"></typeparam>
    public interface ICrudCoralService<TModel, in TSearch, in TOrder> : IServiceWithContext
        where TModel : class, IViewModel, new()
        where TSearch : SearchBase
        where TOrder : OrderBase
    {
        TModel Get(long id);
        void Create(TModel model);
        void Create(List<TModel> models);
        void Modify(TModel model);
        void Modify(List<TModel> model);
        void Remove(long id);
        void Remove(List<long> ids);
        List<TModel> GetAll();
        List<TModel> Search(TSearch search);
        PagedList<TModel> PageSearch(PageInfo page, TSearch search, TOrder order);
    }

    /// <summary>
    /// 默认排序的增删改查服务
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TSearch"></typeparam>
    public interface ICrudCoralService<TModel, in TSearch> : ICrudCoralService<TModel, TSearch, OrderBase>
        where TModel : class, IViewModel, new()
        where TSearch : SearchBase
    {

    }

    /// <summary>
    /// 默认查询的增删改查服务
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public interface ICrudCoralService<TModel> : ICrudCoralService<TModel, SearchBase, OrderBase>
        where TModel : class, IViewModel, new()
    {

    }
}
