using TelesEducacao.Core.DomainObjects;

namespace TelesEducacao.Alunos.Domain;

public class AulaConluida : Entity
{
    public Guid MatriculaId { get; private set; }
    public Guid AulaId { get; private set; }

    public Matricula Matricula { get; private set; } = null!;

    public AulaConluida(Guid matriculaId, Guid aulaId)
    {
        MatriculaId = matriculaId;
        AulaId = aulaId;
    }

    protected AulaConluida()
    { }
}