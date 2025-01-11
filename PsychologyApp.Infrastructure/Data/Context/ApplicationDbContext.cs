using Microsoft.EntityFrameworkCore;
using PsychologyApp.Domain.Entities;

namespace PsychologyApp.Infrastructure.Data.Context;

public class ApplicationDbContext : DbContext
{
    public static string GetPathDB()
    {
        Environment.SpecialFolder folder = Environment.SpecialFolder.LocalApplicationData;
        string folderPath = Environment.GetFolderPath(folder);

        return Path.Join(folderPath, "local.db");
    }

    public ApplicationDbContext() { }

    public void CreateTables()
    {
        _ = Database.EnsureCreated();
    }

    #region Tables
    public DbSet<Quot> Quot { get; set; }
    public DbSet<Technique> Technique { get; set; }
    public DbSet<Reason> Reason { get; set; }
    #endregion

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        _ = options.UseSqlite($"Data Source={GetPathDB()}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
