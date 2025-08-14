using AutoMapper;
using DesafioBackend.Application.DTO;
using DesafioBackend.Domain.DTO;
using DesafioBackend.Domain.Entities;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Moto, MotoReadDTO>();
        CreateMap<MotoCreateDTO, Moto>();
        CreateMap<Entregador, EntregadorReadDTO>();
        CreateMap<EntregadorCreateDTO, Entregador>();
        CreateMap<Locacao, LocacaoReadDTO>();
        CreateMap<LocacaoCreateDTO, Locacao>();
    }
}
