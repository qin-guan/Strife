using System;

namespace Strife.Core.Resources
{
    public interface IResource
    {
        public Guid Id { get; }
        public ResourceType ResourceType { get; }
    }
}