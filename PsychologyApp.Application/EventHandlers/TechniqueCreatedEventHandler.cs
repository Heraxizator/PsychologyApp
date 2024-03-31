using MediatR;
using PsychologyApp.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Application.EventHandlers;

public class TechniqueCreatedEventHandler : INotificationHandler<TechniqueCreatedEvent>
{
    public TechniqueCreatedEventHandler()
    {
    }

    public Task Handle(TechniqueCreatedEvent notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
