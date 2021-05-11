using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Strife.Core.Users;

namespace Strife.API.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class AddStrifeUserAttribute : Attribute, IAsyncResourceFilter
    {
        private readonly UserManager<StrifeUser> _userManager;

        public AddStrifeUserAttribute(UserManager<StrifeUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
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
            
            context.HttpContext.Items["StrifeUser"] = await _userManager.FindByIdAsync(userGuid.ToString());

            await next();
        }
    }
}