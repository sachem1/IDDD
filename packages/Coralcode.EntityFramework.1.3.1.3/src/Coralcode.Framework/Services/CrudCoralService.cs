using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Coralcode.Framework.Data.Core;
using Coralcode.Framework.Data.Specification;
using Coralcode.Framework.Exceptions;
using Coralcode.Framework.Mapper;
using Coralcode.Framework.MessageBus.Event;
using Coralcode.Framework.MessageBus.EventHandlers.Entities;
using Coralcode.Framework.Models;
using Coralcode.Framework.Page;
using EmitMapper.MappingConfiguration;
using Coralcode.Framework.Data.StaticRepository;

namespace Coralcode.Framework.Services
{
    public abstract class CrudCoralService<TEntity, TModel, TSearch, TOrder, TSpecification> : CoralService, ICrudCoralService<TModel, TSearch, TOrder>
        where TEntity : class, IEntity, new()
        where TModel : class, IViewModel, new()
        where TSearch : SearchBase
        where TOrder : OrderBase
        where TSpecification : ISpecification<TEntity>
    {
        protected IRepository<TEntity, TSpecification> Repository;

        /// <summary>
        /// 这里为了隐藏事件总线这一复杂东西
        /// </summary>
        protected IEventBus EventBus;

        protected CrudCoralService(IRepository<TEntity, TSpecification> repository, IEventBus eventBus)
        {
            Repository = repository;
            EventBus = eventBus;
        }

        public virtual TModel Get(long id)
        {
            var entity = Repository.Get(id);
            if (entity == null)
                return null;

            return Convert(entity);
        }

        public virtual void Create(TModel model)
        {
            var entity = Convert(model);
            entity.Id = model.Id;
            Repository.Add(entity);
            model.Id = entity.Id;
            Repository.UnitOfWork.Commit();
            EventBus.Publish(new EntityCreatedEventData<TModel>(model));
        }

        public virtual void Create(List<TModel> models)
        {
            models.ForEach(model =>
            {
                var entity = Convert(model);
                Repository.Add(entity);
                model.Id = entity.Id;
            });
            Repository.UnitOfWork.Commit();
            models.ForEach(model =>
            {
                EventBus.Publish(new EntityCreatedEventData<TModel>(model));
            });


        }

        public virtual void Modify(TModel model)
        {
            var entity = Repository.Get(model.Id);
            if (entity == null)
                throw new NotFoundException("实体未找到");
            var oldModel = Convert(entity);

            Convert(model, entity);
            Repository.Modify(entity);
            Repository.UnitOfWork.Commit();
            EventBus.Publish(new EntityModifyEventData<TModel>(model, oldModel));
        }

        public void Modify(List<TModel> models)
        {
            var items = new List<KeyValuePair<TModel, TModel>>();
            models.ForEach(model =>
            {
                var entity = Repository.Get(model.Id);
                if (entity == null)
                    return;
                var oldModel = Convert(entity);
                items.Add(new KeyValuePair<TModel, TModel>(model, oldModel));
                Convert(model, entity);
                Repository.Modify(entity);
                Repository.UnitOfWork.Commit();
            });
            Repository.UnitOfWork.Commit();
            items.ForEach(item =>
            {
                EventBus.Publish(new EntityModifyEventData<TModel>(item.Key, item.Value));
            });
        }

        public virtual void Remove(long id)
        {
            var item = Get(id);
            if (item == null)
                return;
            Repository.Remove(id);
            Repository.UnitOfWork.Commit();
            EventBus.Publish(new EntityRemoveEventData<TModel>(item));
        }

        public virtual void Remove(List<long> ids)
        {

            var items = new List<TModel>();

            ids.ForEach(id =>
            {
                items.Add(Get(id));
                Repository.Remove(id);
            });

            Repository.UnitOfWork.Commit();
            items.ForEach(item =>
            {
                EventBus.Publish(new EntityRemoveEventData<TModel>(item));
            });
        }

        public virtual List<TModel> GetAll()
        {
            return Repository.GetAll().Select(Convert).ToList();
        }

        public virtual List<TModel> Search(TSearch search)
        {
            return Repository.GetAllMatching(GetFilter(search)).Select(Convert).ToList();
        }

        public PagedList<TModel> PageSearch(PageInfo page, TSearch search, TOrder order)
        {
            return Repository.GetPaged(page.PageIndex, page.PageSize, GetFilter(search),
                new SortExpression<TEntity>(GetOrder(order))).ConvertToPagedList(item => Convert(item));
        }

        protected abstract TSpecification GetFilter(TSearch search);

        protected virtual List<EditableKeyValuePair<Expression<Func<TEntity, dynamic>>, bool>> GetOrder(TOrder order)
        {
            return new List<EditableKeyValuePair<Expression<Func<TEntity, dynamic>>, bool>>
            {
                new EditableKeyValuePair<Expression<Func<TEntity, dynamic>>, bool>(item=>item.Id,true),
            };
        }

        /// <summary>
        /// 出来转换
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected virtual TModel Convert(TEntity entity)
        {
            return DataMapperProvider.Mapper.Convert<TEntity, TModel>(entity);
        }

        /// <summary>
        /// 进入转换
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected virtual TEntity Convert(TModel model)
        {
            return DataMapperProvider.Mapper.Convert<TModel, TEntity>(model, new DefaultMapConfig().IgnoreMembers<TModel, TEntity>(new[] { "Id" }));
        }

        /// <summary>
        /// 属性赋值
        /// </summary>
        /// <param name="model"></param>
        /// <param name="entity"></param>
        protected virtual void Convert(TModel model, TEntity entity)
        {
            DataMapperProvider.Mapper.Convert(model, entity, new DefaultMapConfig().IgnoreMembers<TModel, TEntity>(new[] { "Id" }));
        }
    }

   
}
