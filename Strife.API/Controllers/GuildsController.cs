using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MassTransit;
using AutoMapper;

using Strife.API.Interfaces;
using Strife.API.Filters;
using Strife.API.DTOs;
using Strife.API.Contracts.Events.Guild;

using Strife.Configuration.User;
using Strife.Configuration.Guild;

namespace Strife.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class GuildsController : ControllerBase
    {
        private readonly IGuildService _guildService;
        private readonly IUserService _userService;
        private readonly IPublishEndpoint _publishEndpoint;

        private readonly UserManager<StrifeUser> _userManager;
        private readonly IMapper _mapper;

        public GuildsController(IPublishEndpoint publishEndpoint, UserManager<StrifeUser> userManager, IMapper mapper, IGuildService guildService, IUserService userService)
        {
            _publishEndpoint = publishEndpoint;
            _userManager = userManager;
            _mapper = mapper;

            _guildService = guildService;
            _userService = userService;
        }

        [HttpGet]
        [ServiceFilter(typeof(AddUserDataServiceFilter))]
        public async Task<ActionResult<IEnumerable<GuildDto>>> ReadGuilds()
        {
            var user = (StrifeUser)HttpContext.Items["StrifeUser"];
            var guilds = await _guildService.FindByUserIdAsync(user.Id);

            return Ok(_mapper.Map<IEnumerable<Guild>, IEnumerable<GuildDto>>(guilds));
        }

        [HttpPost]
        [ServiceFilter(typeof(AddUserDataServiceFilter))]
        public async Task<ActionResult<GuildDto>> CreateGuild(GuildDto guildDto)
        {
            var user = (StrifeUser)HttpContext.Items["StrifeUser"];
            var guild = _mapper.Map<GuildDto, Guild>(guildDto);
            guild = await _guildService.AddAsync(guild);

            await _userService.JoinGuildAsync(user, guild);

            return Ok(_mapper.Map<Guild, GuildDto>(guild));
        }
    }
}