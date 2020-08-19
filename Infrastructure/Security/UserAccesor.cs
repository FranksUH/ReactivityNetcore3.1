using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace SecurityInfrastructure.Security
{
    public class UserAccesor : IUserAccesor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserAccesor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetUserName()
        {
            var user = _httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(clm => clm.Type == ClaimTypes.NameIdentifier)?.Value;
            return user;
        }
    }
}
