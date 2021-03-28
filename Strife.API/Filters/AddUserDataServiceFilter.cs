using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;

using Strife.Configuration.User;

namespace Strife.API.Filters
{
    public class AddUserDataServiceFilter : IAsyncActionFilter
    {
        private readonly UserManager<StrifeUser> _userManager;
        public AddUserDataServiceFilter(UserManager<StrifeUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            if (context.HttpContext.User.HasClaim(claim => claim.Type == ClaimTypes.NameIdentifier))
            {
                var user = await _userManager.FindByIdAsync(
                    context.HttpContext.User.Claims.Single(claim =>
                        claim.Type == ClaimTypes.NameIdentifier
                    ).Value
                );

                context.HttpContext.Items.Add("StrifeUser", user);
            }

            await next();
        }

    }
}