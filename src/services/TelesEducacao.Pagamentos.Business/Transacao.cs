using TelesEducacao.Core.DomainObjects;

namespace TelesEducacao.Pagamentos.Business;

public class Transacao : Entity
{
    public Guid MatriculaId { get; set; }
    public Guid PagamentoId { get; set; }
    public decimal Total { get; set; }
    public StatusTransacao StatusTransacao { get; set; }

    // EF. Rel.
    public Pagamento Pagamento { get; set; } = null!;
}