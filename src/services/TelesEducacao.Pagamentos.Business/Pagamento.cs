using TelesEducacao.Core.DomainObjects;

namespace TelesEducacao.Pagamentos.Business;

public class Pagamento : Entity, IAggregateRoot
{
    public Guid MatriculaId { get; set; }
    public decimal Valor { get; set; }

    public DadosCartao DadosCartao { get; set; } = null!;

    // EF. Rel.
    public Transacao Transacao { get; set; } = null!;
}