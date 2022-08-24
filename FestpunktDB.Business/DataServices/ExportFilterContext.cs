using FestpunktDB.Business.Entities;
using FestpunktDB.Business.EntitiesDeleted;
using FestpunktDB.Business.EntitiesImport;
using FestpunktDB.Business;
using Microsoft.EntityFrameworkCore;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace FestpunktDB.Business
{
    public partial class ExportFilterContext : DbContext
    {
        public ExportFilterContext()
        {
        }

        public ExportFilterContext(DbContextOptions<ExportFilterContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Avani> Avani { get; set; }
        public virtual DbSet<GIvlBasis> GIvlBasis { get; set; }
        public virtual DbSet<GIvlKoordinaten> GIvlKoordinaten { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //optionsBuilder.UseJet("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\Users\\Christopher\\source\\repos\\testScaffold\\testScaffold\\Filter_Punkte_Export.accdb");
                optionsBuilder.UseJet(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=..\..\..\..\temp\Filter_Punkte_Export.accdb");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Avani>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("AVANI_FPF_SSR0");

                entity.Property(e => e.Lx)
                    .HasColumnName("LX")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Ly)
                    .HasColumnName("LY")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Pad)
                    .HasColumnName("PAD")
                    .HasMaxLength(255);

                entity.Property(e => e.Part)
                    .HasColumnName("PArt")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<GIvlBasis>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("G_IVL_BASIS");

                entity.HasIndex(e => e.Id)
                    .HasName("ID")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("counter")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.KmAnfang)
                    .HasColumnName("KM_ANFANG")
                    .HasMaxLength(20);

                entity.Property(e => e.KmEnde)
                    .HasColumnName("KM_ENDE")
                    .HasMaxLength(20);

                entity.Property(e => e.KmdbAnfangCh13)
                    .HasColumnName("KMDB_ANFANG_CH13")
                    .HasMaxLength(13);

                entity.Property(e => e.KmdbEndeCh13)
                    .HasColumnName("KMDB_ENDE_CH13")
                    .HasMaxLength(13);

                entity.Property(e => e.Planadresse)
                    .HasColumnName("PLANADRESSE")
                    .HasMaxLength(6);

                entity.Property(e => e.Strecke)
                    .HasColumnName("STRECKE")
                    .HasMaxLength(4);
            });

            modelBuilder.Entity<GIvlKoordinaten>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("G_IVL_KOORDINATEN");

                entity.HasIndex(e => e.Id)
                    .HasName("ID")
                    .IsUnique();

                entity.Property(e => e.BstCh80)
                    .HasColumnName("BST_CH80")
                    .HasMaxLength(80);

                entity.Property(e => e.DgnText)
                    .HasColumnName("DGN_TEXT")
                    .HasMaxLength(6);

                entity.Property(e => e.HSchwerpktGk)
                    .HasColumnName("H_SCHWERPKT_GK")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.HSchwerpktGk3)
                    .HasColumnName("H_SCHWERPKT_GK3")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("counter")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.LsysText)
                    .HasColumnName("LSYS_TEXT")
                    .HasMaxLength(50);

                entity.Property(e => e.RSchwerpktGk)
                    .HasColumnName("R_SCHWERPKT_GK")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.RSchwerpktGk3)
                    .HasColumnName("R_SCHWERPKT_GK3")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Segment).HasMaxLength(3);

                entity.Property(e => e.X1).HasDefaultValueSql("0");

                entity.Property(e => e.X2).HasDefaultValueSql("0");

                entity.Property(e => e.X3).HasDefaultValueSql("0");

                entity.Property(e => e.X4).HasDefaultValueSql("0");

                entity.Property(e => e.X5).HasDefaultValueSql("0");

                entity.Property(e => e.Y1).HasDefaultValueSql("0");

                entity.Property(e => e.Y2).HasDefaultValueSql("0");

                entity.Property(e => e.Y3).HasDefaultValueSql("0");

                entity.Property(e => e.Y4).HasDefaultValueSql("0");

                entity.Property(e => e.Y5).HasDefaultValueSql("0");

                entity.Property(e => e.Zeichenflaeche).HasColumnName("ZEICHENFLAECHE");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
