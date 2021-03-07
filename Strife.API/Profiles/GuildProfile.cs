using System;
using AutoMapper;

using Strife.API.DTOs;
using Strife.API.Converters;

using Strife.Configuration.Guild;

namespace Strife.API.Profiles
{
    public class GuildProfile : Profile
    {
        public GuildProfile()
        {
            CreateMap<Guild, GuildDto>().ConvertUsing<GuildToGuildDto>();
            CreateMap<GuildDto, Guild>().ConvertUsing<GuildDtoToGuild>();
        }
    }
}