using System.ComponentModel.DataAnnotations;

namespace Strife.API.DTOs.Messages
{
    public class CreateMessageRequestDto
    {
        [Required] public string Content { get; set; }
    }
}