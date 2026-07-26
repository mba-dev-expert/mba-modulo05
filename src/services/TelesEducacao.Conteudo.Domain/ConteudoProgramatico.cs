using TelesEducacao.Core.DomainObjects;

namespace TelesEducacao.Conteudo.Domain;

public class ConteudoProgramatico
{
    public string Titulo { get; private set; }
    public string Descricao { get; private set; }

    public ConteudoProgramatico(string titulo, string descricao)
    {
        Titulo = titulo;
        Descricao = descricao;

        Validacoes.ValidarSeVazio(Titulo, "O campo Titulo do conteudo programatico não pode estar vazio");
        Validacoes.ValidarSeVazio(Descricao, "O campo Descricao da conteudo programatico não pode estar vazio");
    }
}