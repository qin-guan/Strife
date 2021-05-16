using System;

namespace Strife.Core.Resources
{
    public class ChildResource
    {
        public Guid ResourceId { get; set; }
        public bool IsWild => ResourceId == Guid.Empty;
        public ResourceType ResourceType { get; set; }
    }
}