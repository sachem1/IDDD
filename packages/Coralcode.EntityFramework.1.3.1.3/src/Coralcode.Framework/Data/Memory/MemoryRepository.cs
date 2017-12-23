using System;
using System.Collections.Generic;
using System.Linq;
using Coralcode.Framework.Data.Core;
using Coralcode.Framework.Data.StaticRepository;
using IUnitOfWork = Coralcode.Framework.Data.Core.IUnitOfWork;
using Coralcode.Framework.Extensions;

namespace Coralcode.Framework.Data.Memory
{
    public class MemeryRepository<TEntity> : BaseStaticRepository<TEntity>, IStaticRepository<TEntity>
        where TEntity : class, IEntity
    {
        protected readonly MemoryUnitOfWork _unitOfWork;
        public MemeryRepository(MemoryUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");

            _unitOfWork = unitOfWork;
        }
        private List<TEntity> _entities;
        protected virtual List<TEntity> getSet()
        {
            return (_entities ?? (_entities = new List<TEntity>()));
        }

        public override void Dispose()
        {
            _entities.Clear();
            _entities = null;
            _unitOfWork.Dispose();
        }

        #region IRepository Members

        public override IUnitOfWork UnitOfWork
        {
            get
            {
                return _unitOfWork;
            }
        }      
        public override void Add(TEntity item)
        {
            //新增如果没有赋值的话则主动赋值，有赋值的 话则使用现有值
            //此判断主要处理跨页面逻辑的问题,比如附件
            if (item.Id == 0)
            {
                item.Id = IdentityGenerator.NewSequentialDomainId();
            }

            _unitOfWork.Register(() =>
            {
                getSet().Add(item);

            });

        }
        

        public override void Remove(TEntity item)
        {
            _unitOfWork.Register(() =>
            {
                getSet().Remove(item);

            });
        }


        public override void Modify(TEntity item)
        { }
        
        public override TEntity Get(long id)
        {
            return id != 0 ? getSet().FirstOrDefault(item => item.Id == id) : null;
        }

        public override IQueryable<TEntity> GetAll()
        {
            return getSet().AsQueryable();
        }
        #endregion



    }
}
