using PsychologyApp.Domain.Common;
using PsychologyApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Domain.Events;

public class TechniqueCreatedEvent : BaseEvent
{
    public TechniqueCreatedEvent(Technique technique)
    {
        this.Technique = technique;
    }

    public Technique Technique { get;}
}
