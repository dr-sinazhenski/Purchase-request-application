using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Shared
{
    public interface IError
    {
        public int ErrorCode { get; init; }
        public string Message { get; init; }
    }
}
