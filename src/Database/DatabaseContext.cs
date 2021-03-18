using DotNetLibraryAdmin.Database.Tables;
using Microsoft.EntityFrameworkCore;

namespace DotNetLibraryAdmin.Database
{
    public class DatabaseContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder ob)
        {
            ob.UseSqlServer(
                $"Data Source={Config.Get("database", "hostname")};" +
                $"Initial Catalog={Config.Get("database", "database")};" +
                $"User ID={Config.Get("database", "username")};" +
                $"Password={Config.Get("database", "password")};");
        }

        #region DbSets

        public DbSet<FileEntry> FileEntries { get; set; }

        public DbSet<Package> Packages { get; set; }

        public DbSet<User> Users { get; set; }

        #endregion
    }
}