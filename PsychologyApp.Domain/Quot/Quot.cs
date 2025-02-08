using PsychologyApp.Domain.Base;
using PsychologyApp.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Domain.Entities;

[Dapper.Contrib.Extensions.Table("Quots")]
public class Quot : Entity
{
    [Key]
    [Dapper.Contrib.Extensions.Key]

    public long QuotId { get; private init; } = default!;
    public string Title { get; private set; } = default!;
    public string Text { get; private set; } = default!;
    public string Theme { get; private set; } = default!;
    public bool IsReaded { get; private set; } = default!;
    public bool IsFavourite {  get; private set; } = default!;

    public static Quot Create(string title, string text, string theme, bool isReaded, bool isFavourite)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);

        Quot quot = new Quot
        {
            Title = title,
            Text = text,
            Theme = theme,
            IsReaded = isReaded,
            IsFavourite = isFavourite
        };

        return quot;
    }

    public void EditTitle(string title) 
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        this.Title = title;
    }

    public void EditText(string text)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        this.Text = text;
    }

    public void MarkAsReaded()
    {
        this.IsReaded = true;
    }

    public void SetFavourite(bool isFavourite)
    {
        this.IsFavourite = isFavourite;
    }
}

