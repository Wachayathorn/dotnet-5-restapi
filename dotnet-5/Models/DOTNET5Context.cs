using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace dotnet_5.Models
{
    public partial class DOTNET5Context : DbContext
    {
        public DOTNET5Context()
        {
        }

        public DOTNET5Context(DbContextOptions<DOTNET5Context> options)
            : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySQL("Server=35.185.190.1;Port=31593;User=root;Password=P@ssw0rd;Database=DOTNET-5");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.Id)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("id");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Fname)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("fname");

                entity.Property(e => e.Lname)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("lname");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
