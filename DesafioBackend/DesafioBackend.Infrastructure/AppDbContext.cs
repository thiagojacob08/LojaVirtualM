using DesafioBackend.Domain;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Moto> Motos { get; set; }
    public DbSet<Entregador> Entregadores { get; set; }
    public DbSet<Locacao> Locacoes { get; set; }
    public DbSet<EventoMoto> EventoMotos { get; set; } // para evento do ano 2024

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Placa da moto única
        modelBuilder.Entity<Moto>()
            .HasIndex(m => m.Placa)
            .IsUnique();

        // CNPJ do entregador único
        modelBuilder.Entity<Entregador>()
            .HasIndex(e => e.CNPJ)
            .IsUnique();

        // CNH número único
        modelBuilder.Entity<Entregador>()
            .HasIndex(e => e.NumeroCNH)
            .IsUnique();

        // Relação entre Locação e Moto e Entregador
        modelBuilder.Entity<Locacao>()
            .HasOne(l => l.Moto)
            .WithMany()
            .HasForeignKey(l => l.MotoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Locacao>()
            .HasOne(l => l.Entregador)
            .WithMany()
            .HasForeignKey(l => l.EntregadorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Outras configurações específicas aqui
    }
}
