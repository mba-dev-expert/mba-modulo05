using System.ComponentModel.DataAnnotations;

namespace TelesEducacao.Bff.Plataforma.Dtos;

public class ConteudoProgramaticoDto
{
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public string Descricao { get; set; } = string.Empty;
}
