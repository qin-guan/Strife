using AutoMapper;
using Strife.API.DTOs.Users;
using Strife.Core.Users;

namespace Strife.API.Profiles
{
    public class UserProfile: Profile
    {
        public UserProfile()
        {
            CreateMap<StrifeUser, UserResponseDto>();
        }
    }
}