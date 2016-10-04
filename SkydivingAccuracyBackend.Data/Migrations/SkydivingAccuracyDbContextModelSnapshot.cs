using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using SkydivingAccuracyBackend.Data.DataAccess;

namespace SkydivingAccuracyBackend.Data.Migrations
{
    [DbContext(typeof(SkydivingAccuracyDbContext))]
    partial class SkydivingAccuracyDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1");

            modelBuilder.Entity("SkydivingAccuracyBackend.Data.DataAccess.WeatherInfo", b =>
                {
                    b.Property<int>("WeatherInfoId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ForecastData");

                    b.Property<string>("Location");

                    b.Property<DateTime>("UpdatedOn");

                    b.HasKey("WeatherInfoId");

                    b.ToTable("WeatherInfos");
                });
        }
    }
}
