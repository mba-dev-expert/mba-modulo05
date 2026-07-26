namespace TelesEducacao.Core.DomainObjects;

public class PagamentoMatricula
{
    public Guid AlunoId { get; set; }
    public Guid CursoId { get; set; }
    public Guid MatriculaId { get; set; }
    public decimal Valor { get; set; }
    public required string NomeCartao { get; set; }
    public required string NumeroCartao { get; set; }
    public required string ExpiracaoCartao { get; set; }
    public required string CvvCartao { get; set; }
}