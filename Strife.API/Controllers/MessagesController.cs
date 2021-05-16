using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Strife.API.Attributes;
using Strife.API.Consumers.Commands.Messages;
using Strife.API.Contracts.Commands.Messages;
using Strife.API.DTOs.Channels;
using Strife.API.DTOs.Messages;
using Strife.API.Permissions;
using Strife.API.Utilities;
using Strife.Core.Database;
using Strife.Core.Guilds;
using Strife.Core.Messages;
using Strife.Core.Resources;

namespace Strife.API.Controllers
{
    [Authorize]
    [ApiController]
    [AddStrifeUserId]
    [Route("Guilds/{guildId:guid}/Channels/{channelId:guid}/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly StrifeDbContext _dbContext;

        public MessagesController(
            IAuthorizationService authorizationService,
            IPublishEndpoint publishEndpoint,
            IMapper mapper,
            ISendEndpointProvider sendEndpointProvider,
            StrifeDbContext dbContext
        )
        {
            _authorizationService = authorizationService;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
            _sendEndpointProvider = sendEndpointProvider;
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedList<MessageResponseDto>>> ReadMessages(
            [FromRoute] Guid guildId,
            [FromRoute] Guid channelId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 30
        )
        {
            try
            {
                var guild = await _dbContext.Guilds.SingleOrDefaultAsync(g => g.Id == guildId);
                if (guild == default(Guild)) return NotFound();

                var authorization = await _authorizationService.AuthorizeAsync(User, guild.Id,
                    new Permission(guild.Id, ResourceType.Message, PermissionOperationType.Read, PermissionAllowDeny.Allow).ToString());
                if (!authorization.Succeeded) return Forbid();

                var messages =
                    await PaginatedList<Message>.CreateAsync(_dbContext.Messages.Where(m => m.ChannelId == channelId),
                        pageNumber, pageSize);

                return Ok(_mapper.Map<IEnumerable<MessageResponseDto>>(messages));
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, "Fatal exception while reading messages");
                return Problem();
            }
        }

        [HttpPost]
        public async Task<ActionResult<Message>> CreateMessage(
            [FromRoute] Guid guildId,
            [FromRoute] Guid channelId,
            [FromBody] CreateMessageRequestDto createMessageRequestDto
        )
        {
            Debug.Assert(HttpContext.Items["StrifeUserId"] != null, "HttpContext.Items['UserId'] != null");
            var userId = (Guid) HttpContext.Items["StrifeUserId"];

            try
            {
                var guild = await _dbContext.Guilds.Include(g => g.Channels).SingleOrDefaultAsync(g => g.Id == guildId);
                if (guild == default(Guild)) return NotFound();
                if (guild.Channels.All(c => c.Id != channelId)) return NotFound();

                var authorization = await _authorizationService.AuthorizeAsync(User, guild.Id,
                    new Permission(guild.Id, ResourceType.Message, PermissionOperationType.Create, PermissionAllowDeny.Allow).ToString());
                if (!authorization.Succeeded) return Forbid();

                var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(MessageAddresses.CreateMessageConsumer);

                var message = _mapper.Map<Message>(createMessageRequestDto);

                await sendEndpoint.Send<ICreateMessage>(new
                {
                    message.Content,
                    GuildId = guildId,
                    ChannelId = channelId,
                    InitiatedBy = userId
                });

                return Ok(_mapper.Map<MessageResponseDto>(message));
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, "Fatal exception while reading messages");
                return Problem();
            }
        }
    }
}