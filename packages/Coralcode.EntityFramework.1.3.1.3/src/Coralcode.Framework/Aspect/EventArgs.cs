using System;

namespace Coralcode.Framework.Aspect
{
    /// <summary>
    /// 泛型事件参数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EventArgs<T> : EventArgs
    {
        public EventArgs()
        {
        }
        public EventArgs(T data)
        {
            this.Data = data;
        }
        public T Data { get; set; }
    }
}
