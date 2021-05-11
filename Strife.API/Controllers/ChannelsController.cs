using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MassTransit;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Strife.API.Attributes;
using Strife.API.Consumers.Commands.Channels;
using Strife.API.Contracts.Commands.Channels;
using Strife.API.DTOs.Channels;
using Strife.API.Permissions;
using Strife.Core.Database;
using Strife.Core.Guilds;
using Strife.Core.Resources;
using Strife.Core.Users;
using Channel = Strife.Core.Channels.Channel;

namespace Strife.API.Controllers
{
    [Authorize]
    [ApiController]
    [AddStrifeUserId]
    [Route("Guilds/{guildId:guid}/[controller]")]
    public class ChannelsController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly UserManager<StrifeUser> _userManager;
        private readonly StrifeDbContext _dbContext;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly IMapper _mapper;

        public ChannelsController(
            IAuthorizationService authorizationService,
            UserManager<StrifeUser> userManager,
            StrifeDbContext dbContext,
            IPublishEndpoint publishEndpoint,
            ISendEndpointProvider sendEndpointProvider,
            IMapper mapper
        )
        {
            _authorizationService = authorizationService;
            _publishEndpoint = publishEndpoint;
            _sendEndpointProvider = sendEndpointProvider;
            _userManager = userManager;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChannelResponseDto>>> ReadChannels(
            [FromRoute] Guid guildId
        )
        {
            try
            {
                var guild = await _dbContext.Guilds.Include(g => g.Channels).SingleOrDefaultAsync(g => g.Id == guildId);
                if (guild == default(Guild)) return NotFound();

                var authorization = await _authorizationService.AuthorizeAsync(User, guild,
                    new Permission(guild.Id, ResourceType.Channel, PermissionAllowDeny.Allow,
                        PermissionOperationType.Read).ToString());
                if (!authorization.Succeeded) return Forbid();

                return Ok(_mapper.Map<IEnumerable<ChannelResponseDto>>(guild.Channels));
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, "Fatal exception while reading channels");
                return Problem();
            }
        }
        
        [HttpGet("{channelId:guid}")]
        public async Task<ActionResult<ChannelResponseDto>> ReadChannel(
            [FromRoute] Guid guildId,
            [FromRoute] Guid channelId
        )
        {
            try
            {
                var guild = await _dbContext.Guilds.Include(g => g.Channels).SingleOrDefaultAsync(g => g.Id == guildId);
                if (guild == default(Guild)) return NotFound();

                var authorization = await _authorizationService.AuthorizeAsync(User, guild,
                    new Permission(guild.Id, ResourceType.Channel, PermissionAllowDeny.Allow,
                        PermissionOperationType.Read).ToString());
                if (!authorization.Succeeded) return Forbid();

                var channel = guild.Channels.Find(c => c.Id == channelId);
                if (channel is null) return NotFound();

                return Ok(_mapper.Map<ChannelResponseDto>(channel));
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, "Fatal exception while reading channels");
                return Problem();
            }
        }

        [HttpPost]
        public async Task<ActionResult<ChannelResponseDto>> CreateChannel(
            [FromRoute] Guid guildId,
            [FromBody] CreateChannelRequestDto createChannelRequestDto
        )
        {
            Debug.Assert(HttpContext.Items["StrifeUserId"] != null, "HttpContext.Items['UserId'] != null");
            var userId = (Guid) HttpContext.Items["StrifeUserId"];

            try
            {
                var guild = await _dbContext.Guilds.FindAsync(guildId);
                if (guild is null) return NotFound();

                var authorization = await _authorizationService.AuthorizeAsync(User, guild,
                    new Permission(guild.Id, ResourceType.Channel, PermissionAllowDeny.Allow,
                        PermissionOperationType.Create).ToString());
                if (!authorization.Succeeded) return Forbid();

                var channel = _mapper.Map<Channel>(createChannelRequestDto);
                channel.Id = Guid.NewGuid();

                var createChannelEndpoint =
                    await _sendEndpointProvider.GetSendEndpoint(ChannelAddresses.CreateChannelConsumer);

                await createChannelEndpoint.Send<ICreateChannel>(new
                {
                    GuildId = guildId,
                    ChannelId = channel.Id,
                    channel.Name,
                    channel.IsVoice,
                    channel.GroupName,
                    InitiatedBy = userId
                });

                return Ok(_mapper.Map<ChannelResponseDto>(channel));
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, "Fatal exception while creating channel");
                return Problem();
            }
        }
    }
}