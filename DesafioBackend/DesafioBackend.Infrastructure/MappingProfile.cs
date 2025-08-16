using AutoMapper;
using DesafioBackend.Application.DTO;
using DesafioBackend.Domain.DTO;
using DesafioBackend.Domain.Entities;

namespace DesafioBackend.Infrastructure;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Moto, MotoReadDTO>();
        CreateMap<MotoCreateDTO, Moto>();
        CreateMap<Entregador, EntregadorReadDTO>();
        CreateMap<EntregadorCreateDTO, Entregador>();
        CreateMap<Locacao, LocacaoReadDTO>();
        CreateMap<LocacaoCreateDTO, Locacao>()
            .ForMember(dest => dest.DataInicio, opt => opt.MapFrom(_ => DateTime.Today))
            .ForMember(dest => dest.DataFimPrevisto, opt => opt.MapFrom(src => DateTime.Today.AddDays(src.PlanoDias)))
            .ForMember(dest => dest.Ativa, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.Multa, opt => opt.MapFrom(_ => 0m))
            .ForMember(dest => dest.ValorDiaria, opt => opt.Ignore())
            .ForMember(dest => dest.ValorTotal, opt => opt.Ignore());
    }
}
