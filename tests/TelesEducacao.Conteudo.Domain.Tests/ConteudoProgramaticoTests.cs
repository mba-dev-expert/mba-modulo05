using TelesEducacao.Core.DomainObjects;

namespace TelesEducacao.Conteudo.Domain.Tests;

public class ConteudoProgramaticoTests
{
    [Fact]
    public void ConteudoProgramatico_Validar_DeveCriarComSucessoQuandoDadosForemValidos()
    {
        // Arrange
        var tituloValido = "Módulo 1: Introdução";
        var descricaoValida = "Visão geral do curso e conceitos básicos.";

        // Act
        var conteudoProgramatico = new ConteudoProgramatico(tituloValido, descricaoValida);

        // Assert
        Assert.Equal(tituloValido, conteudoProgramatico.Titulo);
        Assert.Equal(descricaoValida, conteudoProgramatico.Descricao);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void ConteudoProgramatico_Validar_DeveLancarExcecaoQuandoTituloForVazio(string tituloInvalido)
    {
        // Arrange & Act & Assert
        var ex = Assert.Throws<DomainException>(() =>
            new ConteudoProgramatico(tituloInvalido, "Descrição válida")
        );

        Assert.Equal("O campo Titulo do conteudo programatico não pode estar vazio", ex.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void ConteudoProgramatico_Validar_DeveLancarExcecaoQuandoDescricaoForVazia(string descricaoInvalida)
    {
        // Arrange & Act & Assert
        var ex = Assert.Throws<DomainException>(() =>
            new ConteudoProgramatico("Título válido", descricaoInvalida)
        );

        Assert.Equal("O campo Descricao da conteudo programatico não pode estar vazio", ex.Message);
    }
}