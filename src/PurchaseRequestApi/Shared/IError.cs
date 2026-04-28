using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public interface IError
    {
        public int ErrorCode { get; init; }
        public string Message { get; init; }
    }
}
