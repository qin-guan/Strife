using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
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
using Strife.API.DTOs.Messages;
using Strife.API.Extensions;
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

        private const decimal PageSize = 30;

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
            Debug.Assert(HttpContext.Items["StrifeUserId"] != null, "HttpContext.Items['StrifeUserId'] != null");
            var userId = (Guid) HttpContext.Items["StrifeUserId"];

            try
            {
                var guild = await _dbContext.Guilds.Include(g => g.Channels).SingleOrDefaultAsync(g => g.Id == guildId);
                if (guild == default(Guild)) return NotFound();
                var channels = new List<Channel>();

                var roleClaims = await _dbContext.GetClaimsAsync(guild.Id, userId);
                var guidAndClaimsList = new GuidAndClaimsList(guildId, roleClaims);

                await Task.WhenAll(guild.Channels.Select(async channel =>
                {
                    var permission = new Permission(guild.Id, channel.Id, ResourceType.Channel,
                        PermissionOperationType.Read, PermissionAllowDeny.Allow);
                    var authorization =
                        await _authorizationService.AuthorizeAsync(User, guidAndClaimsList,
                            permission.ToString());
                    if (authorization.Succeeded) channels.Add(channel);
                }));

                return Ok(_mapper.Map<IEnumerable<ChannelResponseDto>>(channels));
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

                var authorization = await _authorizationService.AuthorizeAsync(User, guild.Id,
                    new Permission(guild.Id, channelId, ResourceType.Channel, PermissionOperationType.Read,
                        PermissionAllowDeny.Allow).ToString());
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
        
        [HttpGet("{channelId:guid}/Meta")]
        public async Task<ActionResult<ChannelMetaResponseDto>> ReadMessagesCount(
            [FromRoute] Guid guildId,
            [FromRoute] Guid channelId
        )
        {
            try
            {
                var guild = await _dbContext.Guilds.SingleOrDefaultAsync(g => g.Id == guildId);
                if (guild == default(Guild)) return NotFound();

                var authorization = await _authorizationService.AuthorizeAsync(User, guild.Id,
                    new Permission(guild.Id, ResourceType.Channel, PermissionOperationType.Read,
                        PermissionAllowDeny.Allow, new ChildResource(ResourceType.Message)).ToString());
                if (!authorization.Succeeded) return Forbid();

                var pageCount = Math.Ceiling(await _dbContext.Messages.Where(m => m.ChannelId == channelId).LongCountAsync()/PageSize);

                return Ok(new ChannelMetaResponseDto
                {
                    PageSize = PageSize,
                    PageCount = pageCount
                });
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, "Fatal exception while reading messages count");
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
                var guild = await _dbContext.Guilds.Include(g => g.Channels).SingleOrDefaultAsync(g => g.Id == guildId);
                if (guild == default(Guild)) return NotFound();

                var authorization = await _authorizationService.AuthorizeAsync(User, guild.Id,
                    new Permission(guild.Id, ResourceType.Channel, PermissionOperationType.Create,
                        PermissionAllowDeny.Allow).ToString());
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