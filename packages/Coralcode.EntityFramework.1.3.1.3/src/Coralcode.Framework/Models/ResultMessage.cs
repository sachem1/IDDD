using System.Runtime.Serialization;
using System.Xml.Serialization;
using Coralcode.Framework.Exceptions;

namespace Coralcode.Framework.Models
{
     
    public class BaseMessage
    {
        public BaseMessage() { }
        public BaseMessage(ResultState state, string message = "")
        {
            State = state;
            Message = message;
        }

        
        public ResultState State { get; set; }

        
        public string Message { get; set; }
    }

     
    public class ResultMessage : BaseMessage
    {
        public ResultMessage() { }
        public ResultMessage(ResultState state, string message = "", object data = null)
            : base(state, message)
        {
            Data = data;
        }
        
        public object Data { get; set; }

    }
     
    public class ResultErrorMessage : BaseMessage
    {
        public ResultErrorMessage() { }
        public ResultErrorMessage(ResultState state, string message = "", object data = null, long errorCode = 0)
            : base(state, message)
        {
            Data = data;
            ErrorCode = errorCode;
        }
        public ResultErrorMessage(CoralException excetion)
          : base(ResultState.Fail, excetion.Message)
        {
            //如果出现错误则数据里面也包含错误编码
            //Data = excetion.HResult;
            ErrorCode = excetion.HResult;
        }
        
        public long ErrorCode { get; set; }

        
        public object Data { get; set; }

    }

     
    public class ResultMessage<T> : BaseMessage
    {
        public ResultMessage() { }

        
        public T Data { get; set; }

        public ResultMessage(ResultState state, string message = "", T data = default(T))
            : base(state, message)
        {
            Data = data;
        }
    }

     
    public class ResultErrorMessage<T> : BaseMessage
    {
        public ResultErrorMessage() { }

        
        public T Data { get; set; }

        public ResultErrorMessage(ResultState state, string message = "", T data = default(T), long errorCode = 0)
            : base(state, message)
        {
            Data = data;
            ErrorCode = errorCode;
        }
        public ResultErrorMessage(CoralException excetion, T data = default(T))
          : base(ResultState.Fail, excetion.Message)
        {
            Data = data;
            ErrorCode = excetion.HResult;
        }

        
        public long ErrorCode { get; set; }
    }


    public enum ResultState
    {
        Success =0,
        Fail,
        PartSuccess,
    }
}
