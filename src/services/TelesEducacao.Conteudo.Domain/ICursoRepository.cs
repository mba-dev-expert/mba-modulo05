using TelesEducacao.Core.Data;

namespace TelesEducacao.Conteudo.Domain;

public interface ICursoRepository : IRepository<Curso>
{
    Task<Curso?> ObterPorId(Guid id);

    Task<IEnumerable<Curso>> ObterTodos();

    Task<IEnumerable<Aula>> ObterAulas(Guid cursoId);

    Task<Aula?> ObterAula(Guid aulaId);

    void Adicionar(Curso curso);

    void Atualizar(Curso curso);

    void Remover(Curso curso);

    void AdicionarAula(Aula aula);

    Task RemoverAula(Guid aulaId);
}