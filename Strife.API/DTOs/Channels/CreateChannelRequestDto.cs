using System.ComponentModel.DataAnnotations;

namespace Strife.API.DTOs.Channels
{
    public class CreateChannelRequestDto
    {
        [Required] public string Name { get; set; }
        [Required] public bool IsVoice { get; set; }
        [Required] public string GroupName { get; set; }
    }
}