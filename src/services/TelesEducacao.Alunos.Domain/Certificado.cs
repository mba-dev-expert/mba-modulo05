using TelesEducacao.Core.DomainObjects;

namespace TelesEducacao.Alunos.Domain;

public class Certificado : Entity
{
    public Guid MatriculaId { get; private set; }
    public DateTime DataEmissao { get; private set; }

    public Matricula Matricula { get; private set; } = null!;

    public Certificado(Guid matriculaId)
    {
        MatriculaId = matriculaId;
        DataEmissao = DateTime.UtcNow;
    }
}