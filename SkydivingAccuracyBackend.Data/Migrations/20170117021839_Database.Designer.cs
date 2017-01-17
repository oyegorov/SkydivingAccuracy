using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using SkydivingAccuracyBackend.Data.DataAccess;

namespace SkydivingAccuracyBackend.Data.Migrations
{
    [DbContext(typeof(SkydivingAccuracyDbContext))]
    [Migration("20170117021839_Database")]
    partial class Database
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
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

                    b.Property<string>("Source");

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
