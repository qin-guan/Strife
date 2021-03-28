using System;
using Automatonymous;

namespace Strife.API.Sagas.States
{
    public class CreateGuildState: SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public int State { get; set; }
        public Guid InitiatedBy { get; set; }
    }
}