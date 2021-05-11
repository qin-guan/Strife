using System.ComponentModel.DataAnnotations;

namespace Strife.API.DTOs.Guilds
{
    public class UnsubscribeGuildRequestDto
    {
        [Required] public string ConnectionId { get; set; }
    }
}