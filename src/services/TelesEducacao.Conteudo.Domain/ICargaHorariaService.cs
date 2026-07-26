namespace TelesEducacao.Conteudo.Domain;

public interface ICargaHorariaService : IDisposable
{
    Task<bool> AdicionarCargaHoraria(Guid cursoId, TimeSpan duracao);

    Task<bool> DebitarCargaHoraria(Guid cursoId, TimeSpan duracao);
}