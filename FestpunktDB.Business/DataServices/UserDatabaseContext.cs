using FestpunktDB.Business.Entities;
using Microsoft.EntityFrameworkCore;

namespace FestpunktDB.Business.DataServices
{
    public partial class UserDatabaseContext : DbContext
    {
        public UserDatabaseContext()
        {
        }

        public UserDatabaseContext(DbContextOptions<UserDatabaseContext> options) : base(options)
        {
        }

        public virtual DbSet<Userverwaltung> Userverwaltung { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseJet(
                    @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=..\..\..\..\temp\UserVerwaltung.accdb;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Userverwaltung>(entity =>
            {
                entity.ToTable("Userverwaltung");
                entity.HasKey(e => e.Username);
                entity.Property(e => e.Username).HasColumnName("Username").HasMaxLength(255);
                entity.Property(e => e.Vorname).HasColumnName("Vorname").HasMaxLength(255);
                entity.Property(e => e.Zwischeninitial).HasColumnName("Zwischeninitial").HasMaxLength(255);
                entity.Property(e => e.OE).HasColumnName("OE").HasMaxLength(255);
                entity.Property(e => e.Funktion).HasColumnName("Funktion").HasMaxLength(255);
                entity.Property(e => e.Status).HasColumnName("Status").HasMaxLength(255);
            });
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
