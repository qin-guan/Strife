using System.Collections.Generic;

namespace Strife.API.DTOs.Messages
{
    public class ReadMessagesResponseDto
    {
        public IEnumerable<MessageResponseDto> Messages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
    }
}