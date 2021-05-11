using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MassTransit;
using AutoMapper;
using Serilog;
using Strife.API.Attributes;
using Strife.API.Consumers.Commands.Guilds;
using Strife.API.Contracts.Commands.Guilds;
using Strife.API.Contracts.Events.Guilds;
using Strife.API.Contracts.Events.Hubs;
using Strife.API.DTOs.Guilds;
using Strife.Core.Database;
using Strife.Core.Guilds;

namespace Strife.API.Controllers
{
    [Authorize]
    [ApiController]
    [AddStrifeUserId]
    [Route("[controller]")]
    public class GuildsController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly StrifeDbContext _dbContext;

        public GuildsController(
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
        public async Task<ActionResult<IEnumerable<GuildResponseDto>>> ReadGuilds()
        {
            Debug.Assert(HttpContext.Items["StrifeUserId"] != null, "HttpContext.Items['UserId'] != null");
            var userId = (Guid) HttpContext.Items["StrifeUserId"];

            var guilds = await _dbContext.GuildStrifeUser
                .Where(gsu => gsu.UserId == userId)
                .OrderBy(gsu => gsu.Sequence)
                .Include(gsu => gsu.Guild)
                .Select(gsu => gsu.Guild)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<GuildResponseDto>>(guilds));
        }

        [HttpGet("{guildId:guid}")]
        public async Task<ActionResult<GuildResponseDto>> ReadGuild(
            [FromRoute] Guid guildId
        )
        {
            try
            {
                var guild = await _dbContext.Guilds.FindAsync(guildId);
                if (guild is null) return NotFound();

                return Ok(_mapper.Map<GuildResponseDto>(guild));
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, "Fatal exception while reading Guild details");
                return Problem();
            }
        }

        [HttpPost]
        public async Task<ActionResult<GuildResponseDto>> CreateGuild(
            [FromBody] CreateGuildRequestDto createGuildRequestDto 
        )
        {
            Debug.Assert(HttpContext.Items["StrifeUserId"] != null, "HttpContext.Items['UserId'] != null");
            var userId = (Guid) HttpContext.Items["StrifeUserId"];

            try
            {
                var guild = _mapper.Map<Guild>(createGuildRequestDto);
                guild.Id = Guid.NewGuid();

                var createGuildEndpoint =
                    await _sendEndpointProvider.GetSendEndpoint(GuildAddresses.CreateGuildConsumer);
                
                await createGuildEndpoint.Send<ICreateGuild>(new
                {
                    GuildId = guild.Id,
                    guild.Name,
                    InitiatedBy = userId,
                });

                return Ok(_mapper.Map<GuildResponseDto>(guild));
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, "Fatal exception while creating Guild");
                return Problem();
            }
        }

        [HttpPost("{guildId:guid}/Subscribe")]
        public async Task<ActionResult> Subscribe(
            [FromRoute] Guid guildId,
            [FromBody] SubscribeGuildRequestDto subscribeGuildRequestDto
        )
        {
            Debug.Assert(HttpContext.Items["StrifeUserId"] != null, "HttpContext.Items['UserId'] != null");
            var userId = (Guid) HttpContext.Items["StrifeUserId"];

            try
            {
                await _publishEndpoint.Publish<IGuildSubscribedByUser>(new
                {
                    subscribeGuildRequestDto.ConnectionId,
                    GuildId = guildId,
                    InitiatedBy = userId
                });

                return Ok();
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, "Fatal error while subscribing to Guild");
                return Problem();
            }
        }
    }
}