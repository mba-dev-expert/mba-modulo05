using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TelesEducacao.Auth.Data.Migrations;

// Fixa o provider de design-time em SQL Server para o `dotnet ef`. Sem isso as ferramentas assumem
// o ambiente "Development", onde AddDatabase escolhe SQLite, e as migrations saem com o provider
// errado - origem do PendingModelChangesWarning que derrubava o Auth no cluster.
// A connection string nunca abre conexao: o `migrations add` so precisa do provider configurado.
public class AuthDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
{
    public AuthDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();

        optionsBuilder.UseSqlServer(
            "Server=(localdb)\\mssqllocaldb;Database=TelesEducacaoAuthDb;Trusted_Connection=True;MultipleActiveResultSets=true");

        return new AuthDbContext(optionsBuilder.Options);
    }
}
