using System;
using Microsoft.EntityFrameworkCore;

namespace SkydivingAccuracyBackend.Data.DataAccess
{
    public class SkydivingAccuracyDbContext : DbContext
    {
        private readonly string _dbFileName;

        public SkydivingAccuracyDbContext(): this("skydiving_accuracy")
        {
        }
        
        public SkydivingAccuracyDbContext(string dbFileName)
        {
            if (dbFileName == null)
                throw new ArgumentNullException(nameof(dbFileName));

            _dbFileName = dbFileName;
        }

        public DbSet<WeatherInfo> WeatherInfos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename=./{_dbFileName}");
        }
    }
}
