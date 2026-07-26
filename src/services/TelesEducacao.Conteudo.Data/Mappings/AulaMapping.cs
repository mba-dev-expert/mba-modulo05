using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TelesEducacao.Conteudo.Domain;

namespace TelesEducacao.Conteudo.Data.Mappings;

public class AulaMapping : IEntityTypeConfiguration<Aula>
{
    public void Configure(EntityTypeBuilder<Aula> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(p => p.Conteudo).IsRequired().HasMaxLength(500);
        builder.Property(p => p.Titulo).IsRequired().HasMaxLength(100);

        builder.HasOne(c => c.Curso)
            .WithMany(c => c.Aulas)
            .HasForeignKey(c => c.CursoId);
    }
}