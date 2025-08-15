using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DesafioBackend.Infrastructure;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        // Atenção: use a mesma connection string que está no appsettings.json
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=desafio_backend;Username=postgres;Password=postgres");

        return new AppDbContext(optionsBuilder.Options);
    }
}
