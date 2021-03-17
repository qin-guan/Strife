using System;
using AutoMapper;

using Strife.API.DTOs;

using Strife.Configuration.Guild;

namespace Strife.API.Converters 
{
    public class GuildDtoToGuild: ITypeConverter<GuildDto, Guild>
    {
        public Guild Convert(GuildDto source, Guild destination, ResolutionContext context)
        {
            var isValidGuid = Guid.TryParse(source.Id, out var guid);
            return new Guild
            {
                Id = isValidGuid ? guid : Guid.NewGuid(),
                Name = source.Name,
            };
        }
    }
}