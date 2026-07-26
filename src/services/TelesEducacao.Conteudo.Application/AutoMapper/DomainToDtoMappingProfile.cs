using AutoMapper;
using TelesEducacao.Conteudo.Application.Dtos;
using TelesEducacao.Conteudo.Domain;

namespace TelesEducacao.Conteudo.Application.AutoMapper;

public class DomainToDtoMappingProfile : Profile
{
    public DomainToDtoMappingProfile()
    {
        // CreateMap<Source, Destination>();
        CreateMap<Curso, CursoDto>();
        CreateMap<ConteudoProgramatico, ConteudoProgramaticoDto>();
        CreateMap<Aula, AulaDto>();
    }
}