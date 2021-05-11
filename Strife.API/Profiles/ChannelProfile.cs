using AutoMapper;
using Strife.API.DTOs.Channels;
using Strife.Core.Channels;

namespace Strife.API.Profiles
{
    public class ChannelProfile: Profile
    {
        public ChannelProfile()
        {
            CreateMap<Channel, ChannelResponseDto>();
            CreateMap<CreateChannelRequestDto, Channel>();
        }
    }
}