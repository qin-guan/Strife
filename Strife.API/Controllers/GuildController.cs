using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Strife.API.Contracts.Events.Guild;

namespace Strife.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GuildController : ControllerBase
    {
        private IPublishEndpoint _publishEndpoint;

        public GuildController(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost("create")]
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