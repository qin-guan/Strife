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

using Strife.API.Policies;
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
        public readonly IAuthorizationService _authorizationService;
        private readonly UserManager<StrifeUser> _userManager;

        private readonly IGuildService _guildService;
        private readonly IMapper _mapper;

        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public GuildsController(
            IAuthorizationService authorizationService,
            UserManager<StrifeUser> userManager,
            IPublishEndpoint publishEndpoint,
            IMapper mapper,
            IGuildService guildService,
            ISendEndpointProvider sendEndpointProvider
        )
        {
            _authorizationService = authorizationService;
            _userManager = userManager;

            _mapper = mapper;

            _guildService = guildService;

            _publishEndpoint = publishEndpoint;
            _sendEndpointProvider = sendEndpointProvider;
        }

        [HttpGet]
        [ServiceFilter(typeof(AddUserDataServiceFilter))]
        public async Task<ActionResult<GuildDto[]>> ReadGuilds()
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
            var parsable = Guid.TryParse(guildId, out var guid);
            if (!parsable) return BadRequest();

            var guild = await _guildService.FindByIdAsync(guid);
            return Ok(_mapper.Map<Guild, GuildDto>(guild));
        }

        [HttpGet("{guildId}/Channels")]
        [ServiceFilter(typeof(AddUserDataServiceFilter))]
        public async Task<ActionResult<ChannelDto[]>> ReadChannels(string guildId)
        {
            var parsable = Guid.TryParse(guildId, out var guid);
            if (!parsable) return BadRequest();

            var authorizationResult = await _authorizationService
                .AuthorizeAsync(User, guid, GuildPolicies.ReadChannels);

            if (!authorizationResult.Succeeded) return Forbid();

            return Ok();
        }

        [HttpPost]
        [ServiceFilter(typeof(AddUserDataServiceFilter))]
        public async Task<ActionResult<GuildDto>> CreateGuild(GuildDto guildDto)
        {
            var user = (StrifeUser)HttpContext.Items["StrifeUser"];
            Debug.Assert(user != null, nameof(user) + " != null");

            var guild = _mapper.Map<GuildDto, Guild>(guildDto);

            var endpoints = await Task.WhenAll(new List<Task<ISendEndpoint>>
            {
                _sendEndpointProvider.GetSendEndpoint(new Uri("queue:CreateGuild")),
                _sendEndpointProvider.GetSendEndpoint(new Uri("queue:CreateGuildRole")),
                _sendEndpointProvider.GetSendEndpoint(new Uri("queue:AddGuildUser"))
            });

            await endpoints[0].Send<ICreateGuild>(new
            {
                GuildId = guild.Id,
                Name = guild.Name,
                InitiatedBy = user.Id,
            });

            await Task.WhenAll(new List<Task>
            {
                endpoints[1].Send<ICreateGuildRole>(new
                {
                        GuildId = guild.Id,
                        Name = "everyone",
                        Policies = new List<string>
                            {
                                GuildPolicies.CreateChannels,
                                GuildPolicies.ReadChannels,
                                GuildPolicies.UpdateChannels,
                                GuildPolicies.DeleteChannels,
                            },
                        InitiatedBy = user.Id
                }),
                endpoints[1].Send<ICreateGuildRole>(new
                {
                    GuildId = guild.Id,
                    Name = "__owner",
                }),
                endpoints[2].Send<IAddGuildUser>(new
                {
                    GuildId = guild.Id,
                    InitiatedBy = user.Id
                })
            });

            return Ok(_mapper.Map<Guild, GuildDto>(guild));
        }
    }
}