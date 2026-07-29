using TelesEducacao.Alunos.Domain;

namespace TelesEducacao.Alunos.Domain.Tests;

public class CertificadoTests
{
    [Fact]
    public void Construtor_DeveDefinirMatriculaEDataDeEmissaoUtc()
    {
        // Arrange
        var matriculaId = Guid.NewGuid();
        var inicio = DateTime.UtcNow;

        // Act
        var certificado = new Certificado(matriculaId);
        var fim = DateTime.UtcNow;

        // Assert
        Assert.Equal(matriculaId, certificado.MatriculaId);
        Assert.InRange(certificado.DataEmissao, inicio, fim);
        Assert.Equal(DateTimeKind.Utc, certificado.DataEmissao.Kind);
    }
}
