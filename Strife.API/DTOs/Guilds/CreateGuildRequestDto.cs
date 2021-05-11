using System.ComponentModel.DataAnnotations;

namespace Strife.API.DTOs.Guilds
{
    public class CreateGuildRequestDto
    {
        [Required] public string Name { get; set; }
    }
}