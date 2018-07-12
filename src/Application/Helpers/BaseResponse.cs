using System.Collections.Generic;

using API.Contracts.Dtos;

using Microsoft.Extensions.Primitives;

namespace Application.Helpers
{
    public class BaseResponse
    {
        public BaseResponse(int statusCode, string reason = null)
        {
            HttpStatusCode = statusCode;
            Reason = reason;
        }

        public int HttpStatusCode { get; set; }
        public string Reason { get; set; }
        public KeyValuePair<string, StringValues> s { get; set; }
    }

    public class BaseResponse<T> : BaseResponse
    {
        public BaseResponse(int statusCode, string reason = null) : base(statusCode, reason)
        {
        }

        public BaseResponse(int statusCode, T data, string reason = null) : base(statusCode, reason)
        {
            Data = data;
        }

        public T Data { get; set; }

        public new static BaseResponse<T> NotFound(string reason)
        {
            return new BaseResponse<T>(404, reason);
        }
    }
}
