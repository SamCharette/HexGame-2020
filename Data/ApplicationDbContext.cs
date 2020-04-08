using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("DataSource=" + Data.Properties.Resources.ConnectionString);
        }

        public DbSet<Config> PlayerConfigurations { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Move> Moves { get; set; }
    }
}
