namespace TelesEducacao.Conteudo.Domain;

public class CargaHorariaService : ICargaHorariaService
{
    public ICursoRepository _cursoRepository;

    public CargaHorariaService(ICursoRepository cursoRepository)
    {
        _cursoRepository = cursoRepository;
    }

    public void Dispose()
    {
        _cursoRepository.Dispose();
    }

    public async Task<bool> AdicionarCargaHoraria(Guid cursoId, TimeSpan duracao)
    {
        var curso = await _cursoRepository.ObterPorId(cursoId);
        if (curso == null) return false;
        curso.AdicionarCargaHoraria(duracao);
        _cursoRepository.Atualizar(curso);
        return await _cursoRepository.UnitOfWork.Commit();
    }

    public async Task<bool> DebitarCargaHoraria(Guid cursoId, TimeSpan duracao)
    {
        var curso = await _cursoRepository.ObterPorId(cursoId);
        if (curso == null) return false;
        curso.DebitaCargaHoraria(duracao);
        _cursoRepository.Atualizar(curso);
        return await _cursoRepository.UnitOfWork.Commit();
    }
}