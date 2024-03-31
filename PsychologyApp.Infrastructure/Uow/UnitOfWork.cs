using PsychologyApp.Domain.Common;
using PsychologyApp.Infrastructure.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Infrastructure.Uow;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context)
    {
        this._context = context;
    }

    public async Task<bool> Commit()
    {
        bool success = (await this._context.SaveChangesAsync()) > 0;

        return success;
    }

    public void Dispose()
    {
        this._context.Dispose();
    }

    public Task Rollback()
    {
        return Task.CompletedTask;
    }
}
