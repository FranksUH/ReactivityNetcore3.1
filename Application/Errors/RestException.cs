using System;
using System.Collections.Generic;
using System.Net;
using System.Security.AccessControl;
using System.Text;

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
