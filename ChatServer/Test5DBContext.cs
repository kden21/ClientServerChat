using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ChatServer
{
    public partial class Test5DBContext : DbContext
    {
        public Test5DBContext()
        {
        }

        public Test5DBContext(DbContextOptions<Test5DBContext> options) : base(options)
        {
        }

        public virtual DbSet<Message> Messages { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Server=localhost;Database=Test5DB;Integrated Security=false;UserId=admin;Password=admin");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Message>(entity =>
            {
                entity.ToTable("Message");

                entity.Property(e => e.Id).HasDefaultValueSql("nextval('\"MessageSequences\"'::regclass)");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
