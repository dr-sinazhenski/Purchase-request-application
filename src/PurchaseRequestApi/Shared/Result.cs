using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public class Result
    {
        public bool IsSuccess { get; init; }
        public IError? Error { get; init; }

        protected Result(bool isSuccess, IError? error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new(true, null);
        public static Result Failure(IError error) => new(false, error);
    }

    public class Result<T> : Result
    {
        public T? Data { get; init; }

        protected Result(bool isSuccess, IError? error, T? data) : base(isSuccess, error)
        {
            Data = data;
        }
        public static Result<T> Success(T? data) => new(true, null, data);
        public static Result<T> Failure(IError error, T? data = default) => new(false, error, data);
    }
}
