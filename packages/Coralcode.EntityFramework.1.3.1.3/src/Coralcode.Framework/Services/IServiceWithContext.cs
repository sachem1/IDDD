using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coralcode.Framework.Contexts;

namespace Coralcode.Framework.Services
{


    public interface IServiceWithContext : IService
    {
        void InitContext(AppContext appContext, UserContext userContext, SessionContext sessionContext, PageContext pageContext);

        AppContext AppContext { get; }

        UserContext UserContext { get; }

        SessionContext SessionContext { get; }

        PageContext PageContext { get; }

        
    }
}
