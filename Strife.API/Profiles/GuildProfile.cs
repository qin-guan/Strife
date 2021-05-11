using System;
using AutoMapper;
using Strife.API.DTOs.Guilds;
using Strife.Core.Guilds;

namespace Strife.API.Profiles
{
    public class GuildProfile : Profile
    {
        public GuildProfile()
        {
            CreateMap<Guild, GuildResponseDto>();
            CreateMap<CreateGuildRequestDto, Guild>();
        }
    }
}