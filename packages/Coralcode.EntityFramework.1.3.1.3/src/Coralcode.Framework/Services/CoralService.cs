using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Coralcode.Framework.Aspect.Unity;
using Coralcode.Framework.Contexts;
using Coralcode.Framework.Extensions;

namespace Coralcode.Framework.Services
{
    public class CoralService : IServiceWithContext
    {
        private List<IDisposable> _disposables;

        public virtual void InitContext(AppContext appContext, UserContext userContext, SessionContext sessionContext, PageContext pageContext)
        {
            AppContext = appContext;
            UserContext = userContext;
            SessionContext = sessionContext;
            //初始化 嵌套字段
            GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                     .Where(item => item.FieldType.GetInterface(typeof(IServiceWithContext).Name) != null)
                     .Select(item => item.GetValue(this))
                     .Where(item => item != null)
                     .Cast<IServiceWithContext>().ForEach(item => item.InitContext(AppContext, UserContext, SessionContext, pageContext)); ;
            _disposables = GetType()
                    .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                    .Where(item => item.FieldType.GetInterface(typeof(IDisposable).Name) != null)
                    .Select(item => item.GetValue(this))
                    .Where(item => item != null)
                    .Cast<IDisposable>().ToList();
        }

        public AppContext AppContext { get; private set; }
        public UserContext UserContext { get; private set; }
        public SessionContext SessionContext { get; private set; }
        public PageContext PageContext { get; private set; }



        /// <summary>
        /// 销毁对象
        /// </summary>
        public virtual void Dispose()
        {
            if (_disposables == null || _disposables.Count == 0)
                return;
            _disposables?.ForEach(item =>
            {
                item?.Dispose();
            });
            _disposables?.Clear();
            _disposables = null;

        }
    }
}
