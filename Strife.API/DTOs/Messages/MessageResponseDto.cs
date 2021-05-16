using System;

namespace Strife.API.DTOs.Messages
{
    public class MessageResponseDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime DateSent { get; set; }
        public bool IsEdited { get; set; }
        public DateTime DateEdited { get; set; }
        public Guid SenderId { get; set; }
    }
}