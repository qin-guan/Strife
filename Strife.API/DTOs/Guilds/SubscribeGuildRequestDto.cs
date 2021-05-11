using System.ComponentModel.DataAnnotations;

namespace Strife.API.DTOs.Guilds
{
    public class SubscribeGuildRequestDto
    {
        [Required] public string ConnectionId { get; set; }
    }
}