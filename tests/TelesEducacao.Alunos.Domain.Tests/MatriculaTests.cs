using TelesEducacao.Alunos.Domain;

namespace TelesEducacao.Alunos.Domain.Tests;

public class MatriculaTests
{
    [Fact]
    public void Construtor_DeveCriarMatriculaPendentePagamento()
    {
        // Arrange
        var alunoId = Guid.NewGuid();
        var cursoId = Guid.NewGuid();

        // Act
        var matricula = new Matricula(alunoId, cursoId);

        // Assert
        Assert.Equal(alunoId, matricula.AlunoId);
        Assert.Equal(cursoId, matricula.CursoId);
        Assert.Equal(MatriculaStatus.PendentePagamento, matricula.Status);
        Assert.Empty(matricula.Certificados);
    }

    [Theory]
    [InlineData(MatriculaStatus.Ativa)]
    [InlineData(MatriculaStatus.Cancelada)]
    [InlineData(MatriculaStatus.Concluida)]
    [InlineData(MatriculaStatus.PendentePagamento)]
    public void AtualizarStatus_DeveDefinirNovoStatus(MatriculaStatus novoStatus)
    {
        // Arrange
        var matricula = new Matricula(Guid.NewGuid(), Guid.NewGuid());

        // Act
        matricula.AtualizarStatus(novoStatus);

        // Assert
        Assert.Equal(novoStatus, matricula.Status);
    }
}
