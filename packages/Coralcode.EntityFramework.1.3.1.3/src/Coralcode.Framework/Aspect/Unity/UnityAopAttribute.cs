using System;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Coralcode.Framework.Aspect.Unity
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Interface)]
    public abstract class UnityAopAttribute : HandlerAttribute, ICallHandler, IInterceptionBehavior
    {
        public override ICallHandler CreateHandler(IUnityContainer container)
        {
            return this;
        }

        public System.Collections.Generic.IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        /// <summary>
        /// 调用之后的实现逻辑
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected virtual void OnAfter(IMethodInvocation input)
        {

        }


        /// <summary>
        /// 调用之前的实现逻辑
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected virtual void OnBefore(IMethodInvocation input)
        {

        }

        /// <summary>
        /// 调用出现异常的实现逻辑
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected virtual void OnException(IMethodInvocation input, Exception ex)
        {
            throw ex;
        }

        /// <summary>
        /// 接口注入时候的拦截方法
        /// </summary>
        /// <param name="input"></param>
        /// <param name="nextMethod"></param>
        /// <returns></returns>
        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate nextMethod)
        {
            if (IsHasThisAttribute(input))
            {
                OnBefore(input);
            }
            IMethodReturn result = null;
            try
            {
                result = nextMethod()(input, nextMethod);
            }
            catch (Exception ex)
            {
                OnException(input, ex);
            }

            if (IsHasThisAttribute(input))
            {
                OnAfter(input);
            }
            return result;
        }

        /// <summary>
        /// 虚方法注入的拦截方法
        /// </summary>
        /// <param name="input"></param>
        /// <param name="nextMethod"></param>
        /// <returns></returns>
        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate nextMethod)
        {
            var isHasAttribute = IsHasThisAttribute(input);
            if (isHasAttribute)
            {
                OnBefore(input);
            }
            IMethodReturn result = null;
            try
            {
                result = nextMethod()(input, nextMethod);
            }
            catch (Exception ex)
            {
                OnException(input, ex);
            }

            if (isHasAttribute)
            {
                OnAfter(input);
            }
            return result;
        }

        public bool WillExecute
        {
            get { return true; }
        }

        private bool IsHasThisAttribute(IMethodInvocation input)
        {
            var thisType = this.GetType();
            var classAttr = input.Target.GetType().GetCustomAttribute(thisType);
            if (classAttr == null)
                return false;
            var method = input.Target.GetType()
                .GetMethod(input.MethodBase.Name,
                    input.MethodBase.GetParameters().Select(d => d.ParameterType).ToArray());
            if(method==null)
                return false;
            var methodAttr = method.GetCustomAttribute(thisType);
            return methodAttr != null;
        }
    }
}
