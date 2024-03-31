using MediatR;
using PsychologyApp.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Application.EventHandlers;

public class QuotReadedEventHandler : INotificationHandler<QuotReadedEvent>
{
    public QuotReadedEventHandler()
    {
    }

    public Task Handle(QuotReadedEvent notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
