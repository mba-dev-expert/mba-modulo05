using TelesEducacao.Core.DomainObjects;

namespace TelesEducacao.Conteudo.Domain
{
    public class Curso : Entity, IAggregateRoot
    {
        public string Nome { get; private set; }
        public string Descricao { get; private set; }
        public bool Ativo { get; private set; }
        public decimal Valor { get; private set; }
        public TimeSpan? CargaHoraria { get; set; }

        public ConteudoProgramatico ConteudoProgramatico { get; private set; }
        public List<Aula> Aulas { get; private set; } = new();

        protected Curso()
        { }

        public Curso(string nome, string descricao, bool ativo, decimal valor, ConteudoProgramatico conteudoProgramatico)
        {
            Nome = nome;
            Descricao = descricao;
            Ativo = ativo;
            Valor = valor;
            ConteudoProgramatico = conteudoProgramatico;

            Validar();
        }

        //ad hoc setters
        public void Ativar() => Ativo = true;

        public void Desativar() => Ativo = false;

        public void AlterarNome(string nome)
        {
            Nome = nome;
            Validacoes.ValidarSeVazio(Nome, "O campo Nome do curso não pode estar vazio");
        }

        public void AlterarDescricao(string descricao)
        {
            Descricao = descricao;
            Validacoes.ValidarSeVazio(Descricao, "O campo Descricao do curso não pode estar vazio");
        }

        public void AlterarValor(decimal valor)
        {
            Valor = valor;
            Validacoes.ValidarSeMenorQue(Valor, 1, "O campo Valor do curso não pode se menor igual a 0");
        }

        public void AdicionarCargaHoraria(TimeSpan duracao)
        {
            Validacoes.ValidarSeMenorQue(duracao, TimeSpan.Zero, "A duração não pode ser negativa");
            CargaHoraria += duracao;
        }

        public void DebitaCargaHoraria(TimeSpan duracao)
        {
            Validacoes.ValidarSeMenorQue(duracao, TimeSpan.Zero, "A duração não pode ser negativa");
            CargaHoraria -= duracao;
        }

        public void Validar()
        {
            Validacoes.ValidarSeVazio(Nome, "O campo Nome do curso não pode estar vazio");
            Validacoes.ValidarSeVazio(Descricao, "O campo Descricao do curso não pode estar vazio");
            Validacoes.ValidarSeMenorQue(Valor, 1, "O campo Valor do curso não pode se menor igual a 0");
        }
    }
}