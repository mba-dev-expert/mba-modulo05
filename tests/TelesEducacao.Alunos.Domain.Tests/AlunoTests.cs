using TelesEducacao.Alunos.Domain;

namespace TelesEducacao.Alunos.Domain.Tests;

public class AlunoTests
{
    [Fact]
    public void Construtor_DeveCriarAlunoAtivoComUserIdEMatriculasVazias()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var aluno = new Aluno(userId);

        // Assert
        Assert.Equal(userId, aluno.UserId);
        Assert.True(aluno.Ativo);
        Assert.Empty(aluno.Matriculas);
    }

    [Fact]
    public void Desativar_AlunoAtivo_DeveDefinirAlunoComoInativo()
    {
        // Arrange
        var aluno = new Aluno(Guid.NewGuid());

        // Act
        aluno.Desativar();

        // Assert
        Assert.False(aluno.Ativo);
    }

    [Fact]
    public void Ativar_AlunoInativo_DeveDefinirAlunoComoAtivo()
    {
        // Arrange
        var aluno = new Aluno(Guid.NewGuid());
        aluno.Desativar();

        // Act
        aluno.Ativar();

        // Assert
        Assert.True(aluno.Ativo);
    }

    [Fact]
    public void AdicionarMatricula_DeveAdicionarMatriculaAoAluno()
    {
        // Arrange
        var aluno = new Aluno(Guid.NewGuid());
        var matricula = new Matricula(aluno.Id, Guid.NewGuid());

        // Act
        aluno.AdicionarMatricula(matricula);

        // Assert
        Assert.Same(matricula, Assert.Single(aluno.Matriculas));
    }
}
