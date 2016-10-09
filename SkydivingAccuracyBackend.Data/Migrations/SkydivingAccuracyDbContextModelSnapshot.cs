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

            modelBuilder.Entity("SkydivingAccuracyBackend.Data.DataAccess.WindsAloftForecastDto", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AirportCode");

                    b.Property<string>("AirportName");

                    b.Property<double>("Latitude");

                    b.Property<double>("Longitude");

                    b.Property<DateTime>("UpdatedOn");

                    b.Property<DateTime>("ValidFrom");

                    b.Property<DateTime>("ValidTo");

                    b.Property<string>("WindsAloftData");

                    b.HasKey("Id");

                    b.ToTable("WindsAloft");
                });
        }
    }
}
