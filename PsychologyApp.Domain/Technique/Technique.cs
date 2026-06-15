using PsychologyApp.Domain.Base;

namespace PsychologyApp.Domain.Entities;

public class Technique : Entity
{
    public long TechniqueId { get; private init; }
    public string Number { get; private set; } = default!;
    public string Date { get; private set; } = default!;
    public string Header { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public string Subject { get; private set; } = default!;
    public string Author { get; private set; } = default!;
    public string Algorithm { get; private set; } = default!;
    public string? Image { get; private set; }
    public bool IsCompleted { get; private set; }

    public static Technique Create(long id, string number, string date, string header, string description, string subject, string author, string algorithm, string image)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(number);
        ArgumentException.ThrowIfNullOrWhiteSpace(date);
        ArgumentException.ThrowIfNullOrWhiteSpace(header);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        ArgumentException.ThrowIfNullOrWhiteSpace(subject);
        ArgumentException.ThrowIfNullOrWhiteSpace(author);
        ArgumentException.ThrowIfNullOrWhiteSpace(algorithm);

        return new Technique
        {
            TechniqueId = id,
            Number = number,
            Date = date,
            Header = header,
            Description = description,
            Subject = subject,
            Author = author,
            Algorithm = algorithm,
            Image = image,
            IsCompleted = false
        };
    }

    public void SetNumber(string number)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(number);
        Number = number;
    }

    public void SetDate(string date)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(date);
        Date = date;
    }

    public void SetHeader(string header)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(header);
        Header = header;
    }

    public void SetDescription(string description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        Description = description;
    }

    public void SetAlgorithm(string algorithm)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(algorithm);
        Algorithm = algorithm;
    }

    public void SetAuthor(string author)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(author);
        Author = author;
    }

    public void SetSubject(string subject)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(subject);
        Subject = subject;
    }

    public void SetImage(string? image) => Image = image;

    public void ApplyContent(string number, string date, string header, string description, string subject, string author, string algorithm, string? image)
    {
        SetNumber(number);
        SetDate(date);
        SetHeader(header);
        SetDescription(description);
        SetSubject(subject);
        SetAuthor(author);
        SetAlgorithm(algorithm);
        SetImage(image);
    }

    public void MarkAsCompleted() => IsCompleted = true;
}
