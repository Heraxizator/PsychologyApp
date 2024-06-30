using PsychologyApp.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Domain.Entities;

public class Technique : BaseAuditableEntity
{
    public Technique()
    {

    }

    public Technique(string? number, string? date, string? header, string? describtion, string? subject, string? author, bool isCompleted)
    {
        this.Number = number;
        this.Date = date;
        this.Header = header;
        this.Describtion = describtion;
        this.Subject = subject;
        this.Author = author;
        this.IsCompleted = isCompleted;
    }

    [Key]
    public long TechniqueId { get; private init; }

    public string? Number { get; private set; }
    public string? Date {  get; private set; }
    public string? Header {  get; private set; }
    public string? Describtion {  get; private set; }
    public string? Subject { get; private set; }
    public string? Author { get; private set; }
    public string? Algorithm {  get; private set; }
    public string? Image { get; private set; }
    public bool IsCompleted {  get; private set; }

    public void SetNumber(string number)
    {
        this.Number = number;
    }

    public void SetDate(string date)
    {
        this.Date = date;
    }

    public void SetHeader(string header)
    {
        this.Header = header;
    }

    public void SetDescribtion(string describtion)
    {
        this.Describtion = describtion;
    }

    public void SetAlgorithm(string algorithm)
    {
        this.Algorithm = algorithm;
    }

    public void SetAuthor(string author)
    {
        this.Author = author;
    }

    public void SetSubject(string subject)
    {
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
