using PsychologyApp.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Domain.Entities;

public class Quot : BaseAuditableEntity
{
    public Quot()
    {
    }

    public Quot(string? title, string? text, string? theme, bool isReaded)
    {
        this.Title = title;
        this.Text = text;
        this.Theme = theme;
        this.IsReaded = isReaded;
    }

    public string? Title { get; private set; }
    public string? Text { get; private set; }
    public string? Theme { get; private set; }
    public bool IsReaded {  get; private set; }

    public void SetContent(string? title, string? text, string? theme) 
    { 
        this.Title = title;
        this.Text = text;
        this.Theme = theme;
    }

    public void MarkAsReaded()
    {
        this.IsReaded = true;
    }
}

