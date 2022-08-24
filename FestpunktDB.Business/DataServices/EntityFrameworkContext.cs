using FestpunktDB.Business.Entities;
using FestpunktDB.Business.EntitiesDeleted;
using FestpunktDB.Business.EntitiesImport;
using FestpunktDB.Business.EntitiesExport;
using Microsoft.EntityFrameworkCore;

namespace FestpunktDB.Business
{
    public partial class EntityFrameworkContext : DbContext
    {
        public EntityFrameworkContext()
        {
        }

        public EntityFrameworkContext(DbContextOptions<EntityFrameworkContext> options) : base(options)
        {
        }

        public virtual DbSet<GeloeschtPh> GeloeschtPh { get; set; }
        public virtual DbSet<GeloeschtPl> GeloeschtPl { get; set; }
        public virtual DbSet<GeloeschtPp> GeloeschtPp { get; set; }
        public virtual DbSet<GeloeschtPk> GeloeschtPk { get; set; }
        public virtual DbSet<GeloeschtPs> GeloeschtPs { get; set; }
        public virtual DbSet<ImportPh> ImportPh { get; set; }
        public virtual DbSet<ImportPk> ImportPk { get; set; }
        public virtual DbSet<ImportPl> ImportPl { get; set; }
        public virtual DbSet<ImportPp> ImportPp { get; set; }
        public virtual DbSet<ImportPs> ImportPs { get; set; }
        public virtual DbSet<Ph> Ph { get; set; }
        public virtual DbSet<Pk> Pk { get; set; }
        public virtual DbSet<Pl> Pl { get; set; }
        public virtual DbSet<Pp> Pp { get; set; }
        public virtual DbSet<Ps> Ps { get; set; }
        public virtual DbSet<ExportPh> ExportPh { get; set; }
        public virtual DbSet<ExportPk> ExportPk { get; set; }
        public virtual DbSet<ExportPl> ExportPl { get; set; }
        public virtual DbSet<ExportPp> ExportPp { get; set; }
        public virtual DbSet<ExportPs> ExportPs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseJet(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=..\..\..\..\temp\Datenmodell_FPF_NEU.accdb;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region EntitiesDeleted
            modelBuilder.Entity<GeloeschtPh>(entity =>
            {
                entity.HasKey(e => new {Pad = e.PAD, Hsys = e.HSys}).HasName("P");
                entity.ToTable("geloeschtPH");
                entity.Property(e => e.PAD).HasColumnName("PAD").HasMaxLength(11);
                entity.Property(e => e.HSys).HasColumnName("HSys").HasMaxLength(3);
                entity.Property(e => e.HAuftr).HasColumnName("HAuftr").HasMaxLength(8);
                entity.Property(e => e.HBearb).HasColumnName("HBearb").HasMaxLength(8);
                entity.Property(e => e.HDatum).HasColumnName("HDatum").HasMaxLength(8);
                entity.Property(e => e.HFremd).HasColumnName("HFremd").HasMaxLength(14);
                entity.Property(e => e.HProg).HasColumnName("HProg").HasMaxLength(8);
                entity.Property(e => e.HStat).HasColumnName("HStat").HasMaxLength(1);
                entity.Property(e => e.HText).HasColumnName("HText").HasMaxLength(20);
                entity.Property(e => e.LoeschDatum).HasColumnName("loeschDatum");
                entity.Property(e => e.MH).HasColumnName("MH");
                entity.Property(e => e.MHEXP).HasColumnName("MHEXP");
            });
            modelBuilder.Entity<GeloeschtPk>(entity =>
            {
                entity.HasKey(e => new {Pad = e.PAD, Ksys = e.KSys}).HasName("PrimaryKey");
                entity.ToTable("geloeschtPk");
                entity.HasIndex(e => e.PAD).HasName("PKPAD");
                entity.Property(e => e.PAD).HasColumnName("PAD").HasMaxLength(11);
                entity.Property(e => e.KSys).HasColumnName("KSys");
                entity.Property(e => e.HFremd).HasColumnName("HFremd").HasMaxLength(255);
                entity.Property(e => e.KBearb).HasColumnName("KBearb").HasMaxLength(255);
                entity.Property(e => e.KDatum).HasColumnName("KDatum");
                entity.Property(e => e.KStat).HasColumnName("KStat").HasMaxLength(255);
                entity.Property(e => e.KText).HasColumnName("KText").HasMaxLength(255);
                entity.Property(e => e.LAuftr).HasColumnName("LAuftr").HasMaxLength(255);
                entity.Property(e => e.LoeschDatum).HasColumnName("loeschDatum");
                entity.Property(e => e.LProg).HasColumnName("LProg").HasMaxLength(255);
                entity.Property(e => e.MP).HasColumnName("MP").HasMaxLength(255);
                entity.Property(e => e.MPEXP).HasColumnName("MPEXP").HasMaxLength(255);
                entity.Property(e => e.X).HasMaxLength(255);
                entity.Property(e => e.Y).HasMaxLength(255);
                entity.Property(e => e.Z).HasMaxLength(255);

                //entity.HasOne(d => d.PadNavigation)
                //    .WithMany(p => p.Pk)
                //    .HasForeignKey(d => d.PAD)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("PPPK");
            });
            modelBuilder.Entity<GeloeschtPl>(entity =>
            {
                entity.HasKey(e => new {Pad = e.PAD, Lsys = e.LSys}).HasName("P");
                entity.ToTable("geloeschtPL");
                entity.Property(e => e.PAD).HasColumnName("PAD").HasMaxLength(11);
                entity.Property(e => e.LSys).HasColumnName("LSys").HasMaxLength(3);
                entity.Property(e => e.LAuftr).HasColumnName("LAuftr").HasMaxLength(8);
                entity.Property(e => e.LBearb).HasColumnName("LBearb").HasMaxLength(8);
                entity.Property(e => e.LDatum).HasColumnName("LDatum").HasMaxLength(8);
                entity.Property(e => e.LFremd).HasColumnName("LFremd").HasMaxLength(14);
                entity.Property(e => e.LoeschDatum).HasColumnName("loeschDatum");
                entity.Property(e => e.LProg).HasColumnName("LProg").HasMaxLength(8);
                entity.Property(e => e.LStat).HasColumnName("LStat").HasMaxLength(1);
                entity.Property(e => e.LText).HasColumnName("LText").HasMaxLength(20);
                entity.Property(e => e.MP).HasColumnName("MP");
                entity.Property(e => e.MPEXP).HasColumnName("MPEXP");
            });
            modelBuilder.Entity<GeloeschtPp>(entity =>
            {
                entity.HasKey(e => e.PAD).HasName("PrimaryKey");
                entity.ToTable("geloeschtPp");
                entity.Property(e => e.PAD).HasColumnName("PAD").HasMaxLength(11);
                entity.Property(e => e.Blattschnitt).HasMaxLength(255);
                entity.Property(e => e.LoeschDatum).HasColumnName("loeschDatum");
                entity.Property(e => e.PArt).HasColumnName("PArt").HasMaxLength(4);
                entity.Property(e => e.PAuftr).HasColumnName("PAuftr").HasMaxLength(8);
                entity.Property(e => e.PBearb).HasColumnName("PBearb").HasMaxLength(8);
                entity.Property(e => e.PDatum).HasColumnName("PDatum").HasMaxLength(8);
                entity.Property(e => e.PProg).HasColumnName("PProg").HasMaxLength(8);
                entity.Property(e => e.PText).HasColumnName("PText").HasMaxLength(20);
                entity.Property(e => e.PunktNr).HasDefaultValueSql("0");
            });
            modelBuilder.Entity<GeloeschtPs>(entity =>
            {
                entity.HasKey(e => new {Pad = e.PAD, Pstrecke = e.PStrecke}).HasName("PrimaryKey");
                entity.ToTable("geloeschtPs");
                entity.HasIndex(e => e.PAD).HasName("PSPAD");
                entity.Property(e => e.PAD).HasColumnName("PAD").HasMaxLength(11);
                entity.Property(e => e.PStrecke).HasColumnName("PStrecke").HasMaxLength(4);
                entity.Property(e => e.LoeschDatum).HasColumnName("loeschDatum");
                entity.Property(e => e.PSTRRiKz).HasColumnName("PSTRRiKz");
                entity.Property(e => e.SDatum).HasColumnName("SDatum");

                //entity.HasOne(d => d.PadNavigation)
                //    .WithMany(p => p.Ps)
                //    .HasForeignKey(d => d.PAD)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("PPPS");
            });
            #endregion

            #region EntitiesImport
            modelBuilder.Entity<ImportPh>(entity =>
            {
                entity.HasKey(e => new { Pad = e.PAD, Hsys = e.HSys}).HasName("P");
                entity.ToTable("ImportPH");
                entity.Property(e => e.PAD).HasColumnName("PAD").HasMaxLength(11);
                entity.Property(e => e.HSys).HasColumnName("HSys").HasMaxLength(3);
                entity.Property(e => e.HAuftr).HasColumnName("HAuftr").HasMaxLength(8);
                entity.Property(e => e.HBearb).HasColumnName("HBearb").HasMaxLength(8);
                entity.Property(e => e.HDatum).HasColumnName("HDatum").HasMaxLength(8);
                entity.Property(e => e.HFremd).HasColumnName("HFremd").HasMaxLength(14);
                entity.Property(e => e.HProg).HasColumnName("HProg").HasMaxLength(8);
                entity.Property(e => e.HStat).HasColumnName("HStat").HasMaxLength(1);
                entity.Property(e => e.HText).HasColumnName("HText").HasMaxLength(20);
                entity.Property(e => e.LoeschDatum).HasColumnName("loeschDatum");
                entity.Property(e => e.MH).HasColumnName("MH");
                entity.Property(e => e.MHEXP).HasColumnName("MHEXP")
                ;
                /*entity.HasOne(d => d.ImportPADNavigation).WithMany(p => p.ImportPh).HasForeignKey(d => d.PAD)
                    .OnDelete(DeleteBehavior.ClientSetNull);*/

            });
            modelBuilder.Entity<ImportPk>(entity =>
            {
                entity.HasKey(e => new { Pad = e.PAD, Ksys = e.KSys}).HasName("PrimaryKey");
                entity.ToTable("ImportPK");
                entity.Property(e => e.PAD).HasColumnName("PAD").HasMaxLength(11);
                entity.Property(e => e.KSys).HasColumnName("KSys");
                entity.Property(e => e.HFremd).HasColumnName("HFremd").HasMaxLength(255);
                entity.Property(e => e.KBearb).HasColumnName("KBearb").HasMaxLength(255);
                entity.Property(e => e.KDatum).HasColumnName("KDatum");
                entity.Property(e => e.KStat).HasColumnName("KStat").HasMaxLength(255);
                entity.Property(e => e.KText).HasColumnName("KText").HasMaxLength(255);
                entity.Property(e => e.LAuftr).HasColumnName("LAuftr").HasMaxLength(255);
                entity.Property(e => e.LoeschDatum).HasColumnName("loeschDatum");
                entity.Property(e => e.LProg).HasColumnName("LProg").HasMaxLength(255);
                entity.Property(e => e.MP).HasColumnName("MP").HasMaxLength(255);
                entity.Property(e => e.MPEXP).HasColumnName("MPEXP").HasMaxLength(255);
                entity.Property(e => e.X).HasMaxLength(255);
                entity.Property(e => e.Y).HasMaxLength(255);
                entity.Property(e => e.Z).HasMaxLength(255);
                //entity.HasOne(d => d.ImportPADNavigation).WithMany(p => p.ImportPk).HasForeignKey(d => d.PAD)
                   // .OnDelete(DeleteBehavior.ClientSetNull);


            });
            modelBuilder.Entity<ImportPl>(entity =>
            {
                entity.HasKey(e => new {Pad = e.PAD, Lsys = e.LSys}).HasName("P");
                entity.ToTable("ImportPL");
                entity.Property(e => e.PAD).HasColumnName("PAD").HasMaxLength(11);
                entity.Property(e => e.LSys).HasColumnName("LSys").HasMaxLength(3);
                entity.Property(e => e.LAuftr).HasColumnName("LAuftr").HasMaxLength(8);
                entity.Property(e => e.LBearb).HasColumnName("LBearb").HasMaxLength(8);
                entity.Property(e => e.LDatum).HasColumnName("LDatum").HasMaxLength(8);
                entity.Property(e => e.LFremd).HasColumnName("LFremd").HasMaxLength(14);
                entity.Property(e => e.LoeschDatum).HasColumnName("loeschDatum");
                entity.Property(e => e.LProg).HasColumnName("LProg").HasMaxLength(8);
                entity.Property(e => e.LStat).HasColumnName("LStat").HasMaxLength(1);
                entity.Property(e => e.LText).HasColumnName("LText").HasMaxLength(20);
                entity.Property(e => e.MP).HasColumnName("MP");
                entity.Property(e => e.MPEXP).HasColumnName("MPEXP");
                
            });
            modelBuilder.Entity<ImportPp>(entity =>
            {
                entity.HasKey(e => e.PAD).HasName("PrimaryKey");
                entity.ToTable("ImportPP");
                entity.Property(e => e.PAD).HasColumnName("PAD").HasMaxLength(11);
                entity.Property(e => e.Blattschnitt).HasMaxLength(255);
                entity.Property(e => e.LoeschDatum).HasColumnName("loeschDatum");
                entity.Property(e => e.PArt).HasColumnName("PArt").HasMaxLength(4);
                entity.Property(e => e.PAuftr).HasColumnName("PAuftr").HasMaxLength(8);
                entity.Property(e => e.PBearb).HasColumnName("PBearb").HasMaxLength(8);
                entity.Property(e => e.PDatum).HasColumnName("PDatum").HasMaxLength(8);
                entity.Property(e => e.PProg).HasColumnName("PProg").HasMaxLength(8);
                entity.Property(e => e.PText).HasColumnName("PText").HasMaxLength(20);
                entity.Property(e => e.PunktNr).HasDefaultValueSql("0");
            });
            modelBuilder.Entity<ImportPs>(entity =>
            {
                entity.HasKey(e => new {Pad = e.PAD, Pstrecke = e.PStrecke}).HasName("PrimaryKey");
                entity.ToTable("ImportPS");
                entity.Property(e => e.PAD).HasColumnName("PAD").HasMaxLength(11);
                entity.Property(e => e.PStrecke).HasColumnName("PStrecke").HasMaxLength(4);
                entity.Property(e => e.LoeschDatum).HasColumnName("loeschDatum");
                entity.Property(e => e.PSTRRiKz).HasColumnName("PSTRRiKz");
                entity.Property(e => e.SDatum).HasColumnName("SDatum");
               
            });
            #endregion

            #region Entities
            modelBuilder.Entity<Ph>(entity =>
            {
                entity.HasKey(e => new {e.PAD, Hsys = e.HSys}).HasName("P");
                entity.ToTable("PH");
                entity.HasIndex(e => e.PAD).HasName("PHPAD");
                entity.Property(e => e.PAD).HasColumnName("PAD").HasMaxLength(11);
                entity.Property(e => e.HSys).HasColumnName("HSys").HasMaxLength(3);
                entity.Property(e => e.HAuftr).HasColumnName("HAuftr").HasMaxLength(8);
                entity.Property(e => e.HBearb).HasColumnName("HBearb").HasMaxLength(8);
                entity.Property(e => e.HDatum).HasColumnName("HDatum").HasMaxLength(8);
                entity.Property(e => e.HFremd).HasColumnName("HFremd").HasMaxLength(14);
                entity.Property(e => e.HProg).HasColumnName("HProg").HasMaxLength(8);
                entity.Property(e => e.HStat).HasColumnName("HStat").HasMaxLength(1);
                entity.Property(e => e.HText).HasColumnName("HText").HasMaxLength(20);
                entity.Property(e => e.LoeschDatum).HasColumnName("loeschDatum");
                entity.Property(e => e.MH).HasColumnName("MH");
                entity.Property(e => e.MHEXP).HasColumnName("MHEXP");
                entity.HasOne(d => d.PadNavigation).WithMany(p => p.Ph).HasForeignKey(d => d.PAD)
                    .OnDelete(DeleteBehavior.Cascade).HasConstraintName("PPPH");
            });
            modelBuilder.Entity<Pk>(entity =>
            {
                entity.HasKey(e => new {Pad = e.PAD, Ksys = e.KSys}).HasName("PrimaryKey");
                entity.ToTable("PK");
                entity.HasIndex(e => e.PAD).HasName("PKPAD");
                entity.Property(e => e.PAD).HasColumnName("PAD").HasMaxLength(11);
                entity.Property(e => e.KSys).HasColumnName("KSys");
                entity.Property(e => e.HFremd).HasColumnName("HFremd").HasMaxLength(255);
                entity.Property(e => e.KBearb).HasColumnName("KBearb").HasMaxLength(255);
                entity.Property(e => e.KDatum).HasColumnName("KDatum");
                entity.Property(e => e.KStat).HasColumnName("KStat").HasMaxLength(255);
                entity.Property(e => e.KText).HasColumnName("KText").HasMaxLength(255);
                entity.Property(e => e.LAuftr).HasColumnName("LAuftr").HasMaxLength(255);
                entity.Property(e => e.LoeschDatum).HasColumnName("loeschDatum");
                entity.Property(e => e.LProg).HasColumnName("LProg").HasMaxLength(255);
                entity.Property(e => e.MP).HasColumnName("MP").HasMaxLength(255);
                entity.Property(e => e.MPEXP).HasColumnName("MPEXP").HasMaxLength(255);
                entity.Property(e => e.X).HasMaxLength(255);
                entity.Property(e => e.Y).HasMaxLength(255);
                entity.Property(e => e.Z).HasMaxLength(255);
                entity.HasOne(d => d.PadNavigation).WithMany(p => p.Pk).HasForeignKey(d => d.PAD)
                    .OnDelete(DeleteBehavior.Cascade).HasConstraintName("PPPK");
            });
            modelBuilder.Entity<Pl>(entity =>
            {
                entity.HasKey(e => new {Pad = e.PAD, Lsys = e.LSys}).HasName("P");
                entity.ToTable("PL");
                entity.HasIndex(e => e.PAD).HasName("PLPAD");
                entity.Property(e => e.PAD).HasColumnName("PAD").HasMaxLength(11);
                entity.Property(e => e.LSys).HasColumnName("LSys").HasMaxLength(3);
                entity.Property(e => e.LAuftr).HasColumnName("LAuftr").HasMaxLength(8);
                entity.Property(e => e.LBearb).HasColumnName("LBearb").HasMaxLength(8);
                entity.Property(e => e.LDatum).HasColumnName("LDatum").HasMaxLength(8);
                entity.Property(e => e.LFremd).HasColumnName("LFremd").HasMaxLength(14);
                entity.Property(e => e.LoeschDatum).HasColumnName("loeschDatum");
                entity.Property(e => e.LProg).HasColumnName("LProg").HasMaxLength(8);
                entity.Property(e => e.LStat).HasColumnName("LStat").HasMaxLength(1);
                entity.Property(e => e.LText).HasColumnName("LText").HasMaxLength(20);
                entity.Property(e => e.MP).HasColumnName("MP");
                entity.Property(e => e.MPEXP).HasColumnName("MPEXP");
                entity.HasOne(d => d.PadNavigation).WithMany(p => p.Pl).HasForeignKey(d => d.PAD)
                    .OnDelete(DeleteBehavior.Cascade).HasConstraintName("PPPL");
            });
            modelBuilder.Entity<Pp>(entity =>
            {
                entity.HasKey(e => e.PAD).HasName("PrimaryKey");
                entity.ToTable("PP");
                entity.Property(e => e.PAD).HasColumnName("PAD").HasMaxLength(11);
                entity.Property(e => e.Blattschnitt).HasMaxLength(255);
                entity.Property(e => e.LoeschDatum).HasColumnName("loeschDatum");
                entity.Property(e => e.PArt).HasColumnName("PArt").HasMaxLength(4);
                entity.Property(e => e.PAuftr).HasColumnName("PAuftr").HasMaxLength(8);
                entity.Property(e => e.PBearb).HasColumnName("PBearb").HasMaxLength(8);
                entity.Property(e => e.PDatum).HasColumnName("PDatum").HasMaxLength(8);
                entity.Property(e => e.PProg).HasColumnName("PProg").HasMaxLength(8);
                entity.Property(e => e.PText).HasColumnName("PText").HasMaxLength(20);
                entity.Property(e => e.PunktNr).HasDefaultValueSql("0");
            });
            modelBuilder.Entity<Ps>(entity =>
            {
                entity.HasKey(e => new {Pad = e.PAD, Pstrecke = e.PStrecke}).HasName("PrimaryKey");
                entity.ToTable("PS");
                entity.HasIndex(e => e.PAD).HasName("PSPAD");
                entity.Property(e => e.PAD).HasColumnName("PAD").HasMaxLength(11);
                entity.Property(e => e.PStrecke).HasColumnName("PStrecke").HasMaxLength(4);
                entity.Property(e => e.LoeschDatum).HasColumnName("loeschDatum");
                entity.Property(e => e.PSTRRiKz).HasColumnName("PSTRRiKz");
                entity.Property(e => e.SDatum).HasColumnName("SDatum");
                entity.HasOne(d => d.PadNavigation).WithMany(p => p.Ps).HasForeignKey(d => d.PAD)
                    .OnDelete(DeleteBehavior.Cascade).HasConstraintName("PPPS");
            });
            #endregion

            #region EntitiesExport
            modelBuilder.Entity<ExportPh>(entity =>
            {
                entity.HasKey(e => new { Pad = e.PAD, Hsys = e.HSys }).HasName("P");
                entity.ToTable("ExportPH");
                entity.Property(e => e.PAD).HasColumnName("PAD").HasMaxLength(11);
                entity.Property(e => e.HSys).HasColumnName("HSys").HasMaxLength(3);
                entity.Property(e => e.HAuftr).HasColumnName("HAuftr").HasMaxLength(8);
                entity.Property(e => e.HBearb).HasColumnName("HBearb").HasMaxLength(8);
                entity.Property(e => e.HDatum).HasColumnName("HDatum").HasMaxLength(8);
                entity.Property(e => e.HFremd).HasColumnName("HFremd").HasMaxLength(14);
                entity.Property(e => e.HProg).HasColumnName("HProg").HasMaxLength(8);
                entity.Property(e => e.HStat).HasColumnName("HStat").HasMaxLength(1);
                entity.Property(e => e.HText).HasColumnName("HText").HasMaxLength(20);
                entity.Property(e => e.LoeschDatum).HasColumnName("loeschDatum");
                entity.Property(e => e.MH).HasColumnName("MH");
                entity.Property(e => e.MHEXP).HasColumnName("MHEXP")
                ;
                /*entity.HasOne(d => d.ImportPADNavigation).WithMany(p => p.ImportPh).HasForeignKey(d => d.PAD)
                    .OnDelete(DeleteBehavior.ClientSetNull);*/

            });
            modelBuilder.Entity<ExportPk>(entity =>
            {
                entity.HasKey(e => new { Pad = e.PAD, Ksys = e.KSys }).HasName("PrimaryKey");
                entity.ToTable("ExportPK");
                entity.Property(e => e.PAD).HasColumnName("PAD").HasMaxLength(11);
                entity.Property(e => e.KSys).HasColumnName("KSys");
                entity.Property(e => e.HFremd).HasColumnName("HFremd").HasMaxLength(255);
                entity.Property(e => e.KBearb).HasColumnName("KBearb").HasMaxLength(255);
                entity.Property(e => e.KDatum).HasColumnName("KDatum");
                entity.Property(e => e.KStat).HasColumnName("KStat").HasMaxLength(255);
                entity.Property(e => e.KText).HasColumnName("KText").HasMaxLength(255);
                entity.Property(e => e.LAuftr).HasColumnName("LAuftr").HasMaxLength(255);
                entity.Property(e => e.LoeschDatum).HasColumnName("loeschDatum");
                entity.Property(e => e.LProg).HasColumnName("LProg").HasMaxLength(255);
                entity.Property(e => e.MP).HasColumnName("MP").HasMaxLength(255);
                entity.Property(e => e.MPEXP).HasColumnName("MPEXP").HasMaxLength(255);
                entity.Property(e => e.X).HasMaxLength(255);
                entity.Property(e => e.Y).HasMaxLength(255);
                entity.Property(e => e.Z).HasMaxLength(255);
                //entity.HasOne(d => d.ImportPADNavigation).WithMany(p => p.ImportPk).HasForeignKey(d => d.PAD)
                // .OnDelete(DeleteBehavior.ClientSetNull);


            });
            modelBuilder.Entity<ExportPl>(entity =>
            {
                entity.HasKey(e => new { Pad = e.PAD, Lsys = e.LSys }).HasName("P");
                entity.ToTable("ExportPL");
                entity.Property(e => e.PAD).HasColumnName("PAD").HasMaxLength(11);
                entity.Property(e => e.LSys).HasColumnName("LSys").HasMaxLength(3);
                entity.Property(e => e.LAuftr).HasColumnName("LAuftr").HasMaxLength(8);
                entity.Property(e => e.LBearb).HasColumnName("LBearb").HasMaxLength(8);
                entity.Property(e => e.LDatum).HasColumnName("LDatum").HasMaxLength(8);
                entity.Property(e => e.LFremd).HasColumnName("LFremd").HasMaxLength(14);
                entity.Property(e => e.LoeschDatum).HasColumnName("loeschDatum");
                entity.Property(e => e.LProg).HasColumnName("LProg").HasMaxLength(8);
                entity.Property(e => e.LStat).HasColumnName("LStat").HasMaxLength(1);
                entity.Property(e => e.LText).HasColumnName("LText").HasMaxLength(20);
                entity.Property(e => e.MP).HasColumnName("MP");
                entity.Property(e => e.MPEXP).HasColumnName("MPEXP");

            });
            modelBuilder.Entity<ExportPp>(entity =>
            {
                entity.HasKey(e => e.PAD).HasName("PrimaryKey");
                entity.ToTable("ExportPP");
                entity.Property(e => e.PAD).HasColumnName("PAD").HasMaxLength(11);
                entity.Property(e => e.Blattschnitt).HasMaxLength(255);
                entity.Property(e => e.LoeschDatum).HasColumnName("loeschDatum");
                entity.Property(e => e.PArt).HasColumnName("PArt").HasMaxLength(4);
                entity.Property(e => e.PAuftr).HasColumnName("PAuftr").HasMaxLength(8);
                entity.Property(e => e.PBearb).HasColumnName("PBearb").HasMaxLength(8);
                entity.Property(e => e.PDatum).HasColumnName("PDatum").HasMaxLength(8);
                entity.Property(e => e.PProg).HasColumnName("PProg").HasMaxLength(8);
                entity.Property(e => e.PText).HasColumnName("PText").HasMaxLength(20);
                entity.Property(e => e.PunktNr).HasDefaultValueSql("0");
            });
            modelBuilder.Entity<ExportPs>(entity =>
            {
                entity.HasKey(e => new { Pad = e.PAD, Pstrecke = e.PStrecke }).HasName("PrimaryKey");
                entity.ToTable("ExportPS");
                entity.Property(e => e.PAD).HasColumnName("PAD").HasMaxLength(11);
                entity.Property(e => e.PStrecke).HasColumnName("PStrecke").HasMaxLength(4);
                entity.Property(e => e.LoeschDatum).HasColumnName("loeschDatum");
                entity.Property(e => e.PSTRRiKz).HasColumnName("PSTRRiKz");
                entity.Property(e => e.SDatum).HasColumnName("SDatum");

            });
            #endregion

            OnModelCreatingPartial(modelBuilder);

         
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
