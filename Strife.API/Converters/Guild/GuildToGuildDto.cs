using System;
using AutoMapper;

using Strife.API.DTOs;

using Strife.Configuration.Guild;

namespace Strife.API.Converters 
{
    public class GuildToGuildDto: ITypeConverter<Guild, GuildDto>
    {
        public GuildDto Convert(Guild source, GuildDto destination, ResolutionContext context)
        {
            return new GuildDto
            {
                Id = source.Id.ToString(),
                Name = source.Name,
            };
        }
    }
}