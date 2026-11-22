using ExoApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ExoApi.Data;

public sealed class ExoApiContext(DbContextOptions<ExoApiContext> options) : DbContext(options)
{
    public DbSet<Projeto> Projetos => Set<Projeto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Projeto>(entity =>
        {
            entity.ToTable("Projetos");
            entity.HasKey(project => project.Id);
            entity.HasIndex(project => project.Nome).IsUnique();
            entity.Property(project => project.Nome).HasMaxLength(120).IsRequired();
            entity.Property(project => project.Descricao).HasMaxLength(1000).IsRequired();
            entity.Property(project => project.Status).HasMaxLength(30).IsRequired();
            entity.Property(project => project.CriadoEmUtc).HasColumnType("datetime2");
            entity.Property(project => project.AtualizadoEmUtc).HasColumnType("datetime2");
        });
    }
}
