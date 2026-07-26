using System.ComponentModel.DataAnnotations;

namespace TelesEducacao.Conteudo.Application.Dtos;

public class CursoDto
{
    [Key]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public string Descricao { get; set; } = string.Empty;

    public bool Ativo { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public decimal Valor { get; set; }

    public ConteudoProgramaticoDto ConteudoProgramatico { get; set; } = new();
    public List<AulaDto> Aulas { get; set; } = new();
}

public class CriaCursoDto
{
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public string Descricao { get; set; } = string.Empty;

    public bool Ativo { get; set; } = true;

    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public decimal Valor { get; set; }

    public ConteudoProgramaticoDto ConteudoProgramatico { get; set; } = new();
}

public class AtualizaCursoDto
{
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public Guid Id { get; set; }

    public string? Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; } = string.Empty;
    public bool? Ativo { get; set; }
    public decimal? Valor { get; set; }
}