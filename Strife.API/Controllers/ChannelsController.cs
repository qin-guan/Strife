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
    public class ChannelsController : ControllerBase
    {
        private readonly IGuildService _guildService;
        private readonly IPublishEndpoint _publishEndpoint;

        private readonly UserManager<StrifeUser> _userManager;
        private readonly IMapper _mapper;

        public ChannelsController(IPublishEndpoint publishEndpoint, UserManager<StrifeUser> userManager, IMapper mapper, IGuildService guildService)
        {
            _publishEndpoint = publishEndpoint;
            _userManager = userManager;
            _mapper = mapper;

            _guildService = guildService;
        }
        
    }
}