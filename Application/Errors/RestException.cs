using System;
using System.Net;

namespace Application.Errors
{
    public class RestException : Exception
    {
        public HttpStatusCode Code { get; set; }
        public object Errors { get; set; }

        public RestException(HttpStatusCode statusCode, object errors = null)
        {
            Code = statusCode;
            Errors = errors;
        }
    }
}
