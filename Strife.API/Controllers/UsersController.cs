using System;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Strife.API.Attributes;
using Strife.API.DTOs.Users;
using Strife.API.Permissions;
using Strife.Core.Database;
using Strife.Core.Guilds;
using Strife.Core.Resources;

namespace Strife.API.Controllers
{
    [Authorize]
    [ApiController]
    [AddStrifeUserId]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly StrifeDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authorizationService;

        public UsersController(StrifeDbContext dbContext, IMapper mapper, IAuthorizationService authorizationService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _authorizationService = authorizationService;
        }

        [HttpGet("{userId:guid}")]
        public async Task<ActionResult<UserResponseDto>> ReadUser(
            [FromRoute] Guid userId
        )
        {
            Debug.Assert(HttpContext.Items["StrifeUserId"] != null, "HttpContext.Items['StrifeUserId'] != null");
            var requesterId = (Guid) HttpContext.Items["StrifeUserId"];

            try
            {
                if (userId != requesterId)
                {
                    var gsus = await _dbContext.GuildStrifeUsers.Where(gsu =>
                        gsu.UserId == userId || gsu.UserId == requesterId).Select(g => g.GuildId).ToListAsync();
                    if (gsus.Distinct().Count() == gsus.Count) return Forbid();
                }

                var user = await _dbContext.Users.FindAsync(userId);

                return _mapper.Map<UserResponseDto>(user);
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, "Fatal error while reading user");
                return Problem();
            }
        }
    }
}