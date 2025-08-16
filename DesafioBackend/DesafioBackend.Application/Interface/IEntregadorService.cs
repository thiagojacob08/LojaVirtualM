using DesafioBackend.Application.DTO;
using DesafioBackend.Domain.DTO;

namespace DesafioBackend.Application.Interface;

public interface IEntregadorService
{
    Task<EntregadorReadDTO> CriarEntregadorAsync(EntregadorCreateDTO dto);
    Task<EntregadorReadDTO> GetEntregadorByIdAsync(int id);
    Task<List<EntregadorReadDTO>> GetEntregadoresAsync(string? nome);
    Task SalvarFotoCNHAsync(FotoCNHBase64Dto dto);
}
