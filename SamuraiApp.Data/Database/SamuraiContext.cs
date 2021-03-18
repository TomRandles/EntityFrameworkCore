﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SamuraiApp.Domain;
using System;

namespace SamuraiApp.Data.Database
{
    public class SamuraiContext : DbContext
    {
        public DbSet<Samurai> Samurais { get; set; }
        public DbSet<Quote>  Quotes { get; set; }
        public DbSet<Battle> Battles { get; set; }
        public DbSet<SamuraiBattleStat> SamuraiBattleStats { get; set; }

        protected override void OnConfiguring (DbContextOptionsBuilder builder)
        {
            // Pass connection string to options builder 
            // Include Db logging - write to WriteLine delegate
            // Override default max batch size
            // Enable sensitive data logging
            builder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB; Initial Catalog=SamuraiAppData",
                options => options.MaxBatchSize(100))                           
                   .LogTo(Console.WriteLine, 
                          new[] { DbLoggerCategory.Database.Command.Name,
                                  DbLoggerCategory.Database.Transaction.Name},
                          LogLevel.Debug)
                   .EnableSensitiveDataLogging();                        
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Explicitly implement many-to-many relationship with extra payload
            builder.Entity<Samurai>()
                .HasMany(s => s.Battles)
                .WithMany(b => b.Samurais)
                .UsingEntity<BattleSamurai>             // Use this class to establish the many-to-many relationship
                (bs => bs.HasOne<Battle>().WithMany(),
                  bs => bs.HasOne<Samurai>().WithMany())  // EF core can do as far as this automatically
                .Property(bs => bs.DateJoined)
                .HasDefaultValueSql("getdate()");   // Reference tsql function

            // Horse table has no DbSet. Breaks table naming convention. Use this command to fix.
            // Migration will rename
            builder.Entity<Horse>().ToTable("Horses");

            // Keyless entity to map to a Db view. ToView - stops EF Core from creating a table
            builder.Entity<SamuraiBattleStat>().HasNoKey().ToView("SamuraiBattleStats");
        }
    }
}