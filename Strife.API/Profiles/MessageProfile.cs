using AutoMapper;
using Strife.API.DTOs.Messages;
using Strife.Core.Messages;

namespace Strife.API.Profiles
{
    public class MessageProfile: Profile
    {
        public MessageProfile()
        {
            CreateMap<Message, MessageResponseDto>();
        }
    }
}