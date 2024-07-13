using PsychologyApp.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Application.Services.UserService;

public interface IUserService : IAppService
{
    public Task UnitLocalUser(int cancelTimeout);
    public Task IncrementCompletedCount(int cancelTimeout);
    public Task SetSubscribersCount(int count, int cancelTimeout);
    public Task SetUserInitials(string name, string surname, string patronymic, int cancelTimeout);
}
