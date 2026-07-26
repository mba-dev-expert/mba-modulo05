using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TelesEducacao.Conteudo.Domain;
using TelesEducacao.Core.Data;
using TelesEducacao.Core.Messages;

namespace TelesEducacao.Conteudo.Data;

public class ConteudosContext : DbContext, IUnitOfWork
{
    public ConteudosContext(DbContextOptions<ConteudosContext> options) : base(options)
    {
    }

    public DbSet<Curso> Cursos { get; set; } = null!;
    public DbSet<Aula> Aulas { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //HACK: pra não setar string como varchar(max)
        foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(
            e => e.GetProperties().Where(p => p.ClrType == typeof(string))))
        {
            property.SetMaxLength(100);
        }

        modelBuilder.Ignore<Event>();

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public async Task<bool> Commit()
    {
        return await base.SaveChangesAsync() > 0;
    }
}