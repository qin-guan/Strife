using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
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
using Strife.API.Contracts.Commands.Guild;

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

        private readonly UserManager<StrifeUser> _userManager;
        private readonly IMapper _mapper;

        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public GuildsController(IPublishEndpoint publishEndpoint, UserManager<StrifeUser> userManager, IMapper mapper, IGuildService guildService, ISendEndpointProvider sendEndpointProvider)
        {
            _userManager = userManager;
            _mapper = mapper;

            _guildService = guildService;

            _publishEndpoint = publishEndpoint;
            _sendEndpointProvider = sendEndpointProvider;
        }

        [HttpGet]
        [ServiceFilter(typeof(AddUserDataServiceFilter))]
        public async Task<ActionResult<IEnumerable<GuildDto>>> ReadGuilds()
        {
            var user = (StrifeUser)HttpContext.Items["StrifeUser"];
            Debug.Assert(user != null, nameof(user) + " != null");
            
            var guilds = await _guildService.FindByUserIdAsync(user.Id);

            return Ok(_mapper.Map<IEnumerable<Guild>, IEnumerable<GuildDto>>(guilds));
        }

        [HttpGet("{guildId}")]
        [ServiceFilter(typeof(AddUserDataServiceFilter))]
        public async Task<ActionResult<GuildDto>> ReadGuild(string guildId)
        {
            Guid.TryParse(guildId, out var guid);
            var guild = await _guildService.FindByIdAsync(guid);
            return Ok(_mapper.Map<Guild, GuildDto>(guild));
        }

        [HttpPost]
        [ServiceFilter(typeof(AddUserDataServiceFilter))]
        public async Task<ActionResult<GuildDto>> CreateGuild(GuildDto guildDto)
        {
            var user = (StrifeUser)HttpContext.Items["StrifeUser"];
            Debug.Assert(user != null, nameof(user) + " != null");

            var guild = _mapper.Map<GuildDto, Guild>(guildDto);

            var createEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:CreateGuild"));
            await createEndpoint.Send<ICreateGuild>(new
            {
                GuildId = guild.Id,
                Name = guild.Name,
                InitiatedBy = user.Id,
            });

            var addUserEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:AddGuildUser"));
            await addUserEndpoint.Send<IAddGuildUser>(new
            {
                GuildId = guild.Id,
                InitiatedBy = user.Id
            });

            return Ok(_mapper.Map<Guild, GuildDto>(guild));
        }
    }
}