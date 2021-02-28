using System;
using System.Threading.Tasks;
using MassTransit;
using Strife.API.Contracts.Events.Guild;

namespace Strife.API.Consumers.Guild
{
    public class CreateGuildConsumer: IConsumer<IGuildCreated>
    {
        public Task Consume(ConsumeContext<IGuildCreated> context)
        {
            return Task.CompletedTask;
        }
    }
}