using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Fuji.Models;

namespace Fuji.DAL
{
    public partial class FujiDbContext : DbContext
    {
        public FujiDbContext()
        {
        }

        public FujiDbContext(DbContextOptions<FujiDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Apple> Apples { get; set; } = null!;
        public virtual DbSet<ApplesConsumed> ApplesConsumeds { get; set; } = null!;
        public virtual DbSet<FujiUser> FujiUsers { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=FujiApplicationNET6Connection");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplesConsumed>(entity =>
            {
                entity.HasOne(d => d.Apple)
                    .WithMany(p => p.ApplesConsumeds)
                    .HasForeignKey(d => d.AppleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ApplesConsumed_fk_Apple");

                entity.HasOne(d => d.FujiUser)
                    .WithMany(p => p.ApplesConsumeds)
                    .HasForeignKey(d => d.FujiUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ApplesConsumed_fk_FujiUser");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
