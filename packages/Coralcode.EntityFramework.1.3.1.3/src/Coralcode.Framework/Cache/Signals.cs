using System.Collections.Generic;

namespace Coralcode.Framework.Cache {
    /// <summary>
    /// 标志量
    /// </summary>
    public interface ISignals  {

        /// <summary>
        /// 触发
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="signal"></param>
        void Trigger<T>(T signal);

        /// <summary>
        /// 预定
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="signal"></param>
        /// <returns></returns>
        IVolatileToken When<T>(T signal);
    }

    public class Signals : ISignals {
        readonly IDictionary<object, Token> _tokens = new Dictionary<object, Token>();

        public void Trigger<T>(T signal) {
            lock (_tokens) {
                Token token;
                if (_tokens.TryGetValue(signal, out token)) {
                    _tokens.Remove(signal);
                    token.Trigger();
                }
            }

        }

        public IVolatileToken When<T>(T signal) {
            lock (_tokens) {
                Token token;
                if (!_tokens.TryGetValue(signal, out token)) {
                    token = new Token();
                    _tokens[signal] = token;
                }
                return token;
            }
        }

        class Token : IVolatileToken {
            public Token() {
                IsCurrent = true;
            }
            public bool IsCurrent { get; private set; }
            public void Trigger() { IsCurrent = false; }
        }
    }
}
