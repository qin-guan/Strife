using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MassTransit;

using Strife.API.Contracts.Events.Guild;

namespace Strife.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class GuildController : ControllerBase
    {
        private IPublishEndpoint _publishEndpoint;

        public GuildController(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        [HttpGet("Guilds")]
        public ActionResult ReadGuilds()
        {
            return Ok();
        }

        [HttpPost("Create")]
        public ActionResult CreateGuild(string name)
        {
            _publishEndpoint.Publish<IGuildCreated>(new
            {
                GuildId = Guid.NewGuid()
            });

            return Ok();
        }
    }
}