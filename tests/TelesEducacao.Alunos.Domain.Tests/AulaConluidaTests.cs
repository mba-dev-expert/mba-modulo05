using TelesEducacao.Alunos.Domain;

namespace TelesEducacao.Alunos.Domain.Tests;

public class AulaConluidaTests
{
    [Fact]
    public void Construtor_DeveDefinirMatriculaEAula()
    {
        // Arrange
        var matriculaId = Guid.NewGuid();
        var aulaId = Guid.NewGuid();

        // Act
        var aulaConcluida = new AulaConluida(matriculaId, aulaId);

        // Assert
        Assert.Equal(matriculaId, aulaConcluida.MatriculaId);
        Assert.Equal(aulaId, aulaConcluida.AulaId);
    }
}
