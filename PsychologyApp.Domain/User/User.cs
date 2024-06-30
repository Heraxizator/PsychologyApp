using PsychologyApp.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Domain.Entities;

public class User : BaseAuditableEntity
{
    public User()
    {
    }

    public User(string? userName, string? userSurname, string? userPatronymic,
        int techniquesCompletedCount, int subscribersCount)
    {
        this.UserName = userName;
        this.UserSurname = userSurname;
        this.UserPatronymic = userPatronymic;
        this.TechniquesCompletedCount = techniquesCompletedCount;
        this.SubscribersCount = subscribersCount;
    }

    [Key]
    public long UserId { get; private init; }

    public string? UserName { get; private set; }
    public string? UserSurname { get; private set; }
    public string? UserPatronymic { get; private set; }
    public int TechniquesCompletedCount {  get; private set; }
    public int SubscribersCount { get; private set; }

    public void IncrementTechniquesCompletedCount()
    {
        this.TechniquesCompletedCount++;
    }

    public void SetUserInitials(string? name, string? surname, string? patronymic)
    {
        this.UserName = name;
        this.UserSurname = surname;
        this.UserPatronymic = patronymic;
    }

    public void SetSubscribersCount(int count)
    {
        this.SubscribersCount = count;
    }
}