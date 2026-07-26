using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelesEducacao.Core.DomainObjects;

namespace TelesEducacao.Conteudo.Domain.Tests;

public class AulaTests
{
    [Fact]
    public void Aula_Validar_DeveCriarComSucessoQuandoDadosForemValidos()
    {
        // Arrange
        var tituloValido = "Aula 01: Introdução ao C#";
        var conteudoValido = "Conteúdo detalhado da aula inicial.";
        var cursoIdValido = Guid.NewGuid();

        // Act
        var aula = new Aula(tituloValido, conteudoValido, cursoIdValido);

        // Assert
        Assert.Equal(tituloValido, aula.Titulo);
        Assert.Equal(conteudoValido, aula.Conteudo);
        Assert.Equal(cursoIdValido, aula.CursoId);
        Assert.Empty(aula.MaterialApoio);
    }

    [Fact]
    public void Aula_Validar_DeveLancarExcecaoQuandoCursoIdForVazio()
    {
        // Arrange & Act & Assert
        var ex = Assert.Throws<DomainException>(() =>
            new Aula("Título Válido", "Conteúdo Válido", Guid.Empty)
        );

        Assert.Equal("O campo CursoId da aula não pode estar vazio", ex.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Aula_Validar_DeveLancarExcecaoQuandoTituloForVazio(string tituloInvalido)
    {
        // Arrange & Act & Assert
        var ex = Assert.Throws<DomainException>(() =>
            new Aula(tituloInvalido, "Conteúdo Válido", Guid.NewGuid())
        );

        Assert.Equal("O campo Titulo da aula não pode estar vazio", ex.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Aula_Validar_DeveLancarExcecaoQuandoConteudoForVazio(string conteudoInvalido)
    {
        // Arrange & Act & Assert
        var ex = Assert.Throws<DomainException>(() =>
            new Aula("Título Válido", conteudoInvalido, Guid.NewGuid())
        );

        Assert.Equal("O campo Conteudo da aula não pode estar vazio", ex.Message);
    }

    [Fact]
    public void Aula_AdicionarMaterialApoio_DeveAdicionarItemALista()
    {
        // Arrange
        var aula = new Aula("Título Válido", "Conteúdo Válido", Guid.NewGuid());
        var linkMaterial = "https://teleseducacao.com/material.pdf";

        // Act
        aula.AdicionarMaterialApoio(linkMaterial);

        // Assert
        Assert.Single(aula.MaterialApoio);
        Assert.Contains(linkMaterial, aula.MaterialApoio);
    }

    [Fact]
    public void Aula_AlterarTitulo_DeveAtualizarQuandoValido()
    {
        // Arrange
        var aula = new Aula("Título Antigo", "Conteúdo Válido", Guid.NewGuid());
        var novoTitulo = "Novo Título";

        // Act
        aula.AlterarTitulo(novoTitulo);

        // Assert
        Assert.Equal(novoTitulo, aula.Titulo);
    }

    [Fact]
    public void Aula_AlterarConteudo_DeveAtualizarQuandoValido()
    {
        // Arrange
        var aula = new Aula("Título Válido", "Conteúdo Antigo", Guid.NewGuid());
        var novoConteudo = "Novo Conteúdo";

        // Act
        aula.AlterarConteudo(novoConteudo);

        // Assert
        Assert.Equal(novoConteudo, aula.Conteudo);
    }
}