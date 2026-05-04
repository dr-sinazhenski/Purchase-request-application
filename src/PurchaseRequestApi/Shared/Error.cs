using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public class Error : IError
    {
        public int ErrorCode { get; init; }
        public string Message { get; init; }

        public Error(int code, string message)
        {
            ErrorCode = code;
            Message = message;
        }
        public override string ToString()
        {
            return ErrorCode.ToString() + " " + Message;
        }
    }
}
