using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PsychologyApp.Domain.Common;
using PsychologyApp.Infrastructure.Data.Repositories.Quots;
using PsychologyApp.Infrastructure.Data.Repositories.Reasons;
using PsychologyApp.Infrastructure.Data.Repositories.Techniques;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Infrastructure.Data.Context;

public static class Database
{
    private static readonly SqliteConnection _connection = new(ApplicationDbContext.GetPathDB());

    #region Repositories
    public static readonly QuotRepository QuotRepository = new(_connection);

    public static readonly ReasonRepository ReasonRepository = new(_connection);

    public static readonly TechniqueRepository TechniqueRepository = new(_connection);
    #endregion

    static Database()
    {
        ApplicationDbContext context = new();

        context.Database.EnsureCreated();

        context.Database.CloseConnection();
    }
}
