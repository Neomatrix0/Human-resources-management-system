using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Gestionale_Personale_mvc.Models;

public partial class Database : DbContext
{
    public Database()
    {
    }

    public Database(DbContextOptions<Database> options)
        : base(options)
    {
    }

    public virtual DbSet<Dipendente> Dipendentes { get; set; }

    public virtual DbSet<Indicatori> Indicatoris { get; set; }

    public virtual DbSet<Mansione> Mansiones { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlite("Data Source=database.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Dipendente>(entity =>
        {
            entity.ToTable("dipendente");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Cognome).HasColumnName("cognome");
            entity.Property(e => e.DataDiNascita)
                .HasColumnType("DATE")
                .HasColumnName("dataDiNascita");
            entity.Property(e => e.IndicatoriId).HasColumnName("indicatoriId");
            entity.Property(e => e.Mail).HasColumnName("mail");
            entity.Property(e => e.MansioneId).HasColumnName("mansioneId");
            entity.Property(e => e.Nome).HasColumnName("nome");

            entity.HasOne(d => d.Indicatori).WithMany(p => p.Dipendentes).HasForeignKey(d => d.IndicatoriId);

            entity.HasOne(d => d.Mansione).WithMany(p => p.Dipendentes).HasForeignKey(d => d.MansioneId);
        });

        modelBuilder.Entity<Indicatori>(entity =>
        {
            entity.ToTable("indicatori");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Fatturato)
                .HasColumnType("DOUBLE")
                .HasColumnName("fatturato");
            entity.Property(e => e.Presenze).HasColumnName("presenze");
        });

        modelBuilder.Entity<Mansione>(entity =>
        {
            entity.ToTable("mansione");

            entity.HasIndex(e => e.Titolo, "IX_mansione_titolo").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Stipendio).HasColumnName("stipendio");
            entity.Property(e => e.Titolo).HasColumnName("titolo");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
