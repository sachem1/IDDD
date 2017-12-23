using System;

namespace Coralcode.Framework.Cache {

    public interface IAcquireContext {
        /// <summary>
        /// 监控
        /// </summary>
        Action<IVolatileToken> Monitor { get; }
    }

    public class AcquireContext : IAcquireContext {
        public AcquireContext(string key, Action<IVolatileToken> monitor) {
            Key = key;
            Monitor = monitor;
        }

        public string Key { get; private set; }
        public Action<IVolatileToken> Monitor { get; private set; }
    }
}
