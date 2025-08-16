using DesafioBackend.Application.DTO;
using DesafioBackend.Domain.DTO;
using Microsoft.AspNetCore.Mvc;

namespace DesafioBackend.Application.Interface;

public interface ILocacaoService
{
    Task<LocacaoReadDTO> CriarLocacaoAsync(LocacaoCreateDTO locacaodto);
    Task<LocacaoReadDTO> GetLocacaoById(int idLocacao);
    Task<LocacaoReadDTO> FinalizarLocacaoAsync(int idLocacao);
}
