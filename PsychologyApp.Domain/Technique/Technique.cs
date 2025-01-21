using PsychologyApp.Domain.Base;
using PsychologyApp.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Domain.Entities;

[Dapper.Contrib.Extensions.Table("Techniques")]
public class Technique : Entity
{
    [Key]
    [Dapper.Contrib.Extensions.Key]

    public long TechniqueId { get; private init; } = default!;
    public string Number { get; private set; } = default!;
    public string Date {  get; private set; } = default!;
    public string Header {  get; private set; } = default!;
    public string Describtion {  get; private set; } = default!;
    public string Subject { get; private set; } = default!;
    public string Author { get; private set; } = default!;
    public string Algorithm {  get; private set; } = default!;
    public string? Image { get; private set; } = default!;
    public bool IsCompleted {  get; private set; }

    public static Technique Create(long id, string number, string date, string header, string describtion, string subject, string author, string algorithm, string image)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(number);
        ArgumentException.ThrowIfNullOrWhiteSpace(date);
        ArgumentException.ThrowIfNullOrWhiteSpace(header);
        ArgumentException.ThrowIfNullOrWhiteSpace(describtion);
        ArgumentException.ThrowIfNullOrWhiteSpace(subject);
        ArgumentException.ThrowIfNullOrWhiteSpace(author);
        ArgumentException.ThrowIfNullOrWhiteSpace(algorithm);

        Technique technique = new Technique
        {
            TechniqueId = id,
            Number = number,
            Date = date,
            Header = header,
            Describtion = describtion,
            Subject = subject,
            Author = author,
            Algorithm = algorithm,
            Image = image,
            IsCompleted = false
        };

        return technique;
    }

    public void SetNumber(string number)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(number);
        this.Number = number;
    }

    public void SetDate(string date)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(date);
        this.Date = date;
    }

    public void SetHeader(string header)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(header);
        this.Header = header;
    }

    public void SetDescribtion(string describtion)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(describtion);
        this.Describtion = describtion;
    }

    public void SetAlgorithm(string algorithm)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(algorithm);
        this.Algorithm = algorithm;
    }

    public void SetAuthor(string author)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(author);
        this.Author = author;
    }

    public void SetSubject(string subject)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(subject);
        this.Subject = subject;
    }

    private void SetImage(string image)
    {
        this.Image = image;
    }

    public void MarkAsCompleted()
    {
        this.IsCompleted = true;
    }

}
