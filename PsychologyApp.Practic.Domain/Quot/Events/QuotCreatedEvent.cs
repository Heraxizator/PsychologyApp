﻿using PsychologyApp.Domain.Common;
using PsychologyApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Domain.Events;

public class QuotCreatedEvent : BaseEvent
{
    public QuotCreatedEvent(Quot quot)
    {
        this.Quot = quot;
    }

    public Quot Quot { get;}
}