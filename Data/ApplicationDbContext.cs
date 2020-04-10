using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IConfiguration appConfiguration;
        public ApplicationDbContext(IConfiguration config)
        {
            appConfiguration = config;
        }

        public ApplicationDbContext()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("Config.json");

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("Config.json", true, true)
                .Build();

            appConfiguration = config;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("DataSource=" + appConfiguration["DataSource"]);
        }

        public DbSet<Config> PlayerConfigurations { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Move> Moves { get; set; }
        public DbSet<GamePlayer> GamePlayers { get; set; }
        public DbSet<Monitor> Monitors { get; set; }
    }
}
