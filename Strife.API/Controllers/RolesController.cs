using System;
using System.Diagnostics;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Strife.API.Attributes;

namespace Strife.API.Controllers
{
    [Authorize]
    [ApiController]
    [AddStrifeUserId]
    [Route("[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public RolesController(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }
    }
}