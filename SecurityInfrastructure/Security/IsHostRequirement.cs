using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SecurityInfrastructure.Security
{
    public class IsHostRequirement: IAuthorizationRequirement
    {}

    public class IsHostRequirementHandler : AuthorizationHandler<IsHostRequirement>
    {
        private IHttpContextAccessor _contextAccessor;
        private DataContext _dataContext;
        public IsHostRequirementHandler(IHttpContextAccessor contextAccessor, DataContext dataContext)
        {
            _dataContext = dataContext;
            _contextAccessor = contextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsHostRequirement requirement)
        {
            if (context.Resource is AuthorizationFilterContext authContext)
            {
                var userName = _contextAccessor.HttpContext.User?.Claims?
                    .FirstOrDefault(cl => cl.Type == ClaimTypes.NameIdentifier).Value;

                var activityId = authContext.RouteData.Values["Id"].ToString();
                var activity = _dataContext.Activities.FindAsync(activityId).Result;
                var host = activity.UserActivities.FirstOrDefault(ua => ua.IsHost);

                if (host?.AppUser?.UserName == userName)
                    context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
            return Task.CompletedTask;
        }
    }
}
