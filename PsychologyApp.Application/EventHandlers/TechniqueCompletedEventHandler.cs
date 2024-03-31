using MediatR;
using PsychologyApp.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Application.EventHandlers;

public class TechniqueCompletedEventHandler : INotificationHandler<TechniqueCompletedEvent>
{
    public TechniqueCompletedEventHandler()
    {
    }

    public Task Handle(TechniqueCompletedEvent notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
