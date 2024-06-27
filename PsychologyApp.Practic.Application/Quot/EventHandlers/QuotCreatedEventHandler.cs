using MediatR;
using PsychologyApp.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Application.EventHandlers;

public class QuotCreatedEventHandler : INotificationHandler<QuotCreatedEvent>
{
    public QuotCreatedEventHandler()
    {
    }

    public Task Handle(QuotCreatedEvent notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
