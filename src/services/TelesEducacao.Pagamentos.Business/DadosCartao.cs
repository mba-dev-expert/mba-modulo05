namespace TelesEducacao.Pagamentos.Business;

public class DadosCartao
{
    public string Nome { get; set; } = null!;
    public string Numero { get; set; } = null!;
    public string Expiracao { get; set; } = null!;
    public string Cvv { get; set; } = null!;
}