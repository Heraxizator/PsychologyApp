using Microsoft.EntityFrameworkCore;
using PsychologyApp.Domain.Common;
using PsychologyApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Infrastructure.Share;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly string _dbPath;
    public ApplicationDbContext()
    {
        Environment.SpecialFolder folder = Environment.SpecialFolder.LocalApplicationData;
        string path = Environment.GetFolderPath(folder);
        this._dbPath = Path.Join(path, "local.db");

        try
        {
            this.Database.EnsureCreated();
        }
        
        catch 
        {
            Console.WriteLine();
        }
    }

    #region Tables
    public DbSet<Quot> QuotsTable { get; set; }
    public DbSet<User> UsersTable { get; set; }
    public DbSet<Technique> TechniquesTable { get; set; }
    public DbSet<Reason> ReasonsTable { get; set; }

    #endregion

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        _ = options.UseSqlite($"Data Source={this._dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
