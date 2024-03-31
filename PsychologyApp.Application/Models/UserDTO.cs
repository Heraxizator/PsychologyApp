using PsychologyApp.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Application.Models;

public class UserDTO : BaseDTO
{
    public string? UserName { get; set; }
    public string? UserSurname { get; set; }
    public string? UserPatronymic { get; set; }
    public int TechniquesCompletedCount { get; set; }
    public int SubscribersCount { get; set; }
}
