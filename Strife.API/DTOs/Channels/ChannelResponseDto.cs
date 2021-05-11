using System;

namespace Strife.API.DTOs.Channels
{
    public class ChannelResponseDto 
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string GroupName { get; set; }
        public bool IsVoice { get; set; }
    }
}