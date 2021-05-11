using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Strife.API.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class AddStrifeUserIdAttribute : Attribute, IResourceFilter
    {
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var userId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new UnauthorizedAccessException();
            }

            if (!Guid.TryParse(userId, out var userGuid))
            {
                throw new UnauthorizedAccessException();
            }

            context.HttpContext.Items["StrifeUserId"] = userGuid;
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }
    }
}