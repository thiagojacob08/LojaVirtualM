using DesafioBackend.Application.DTO;

namespace DesafioBackend.Application.Interface;

public interface IMotoService
{
    Task<MotoReadDTO> CriarMotoAsync(MotoCreateDTO dto);
    Task<List<MotoReadDTO>> ListarMotosAsync(string? placa = null);
    Task<MotoReadDTO?> ObterMotoPorIdAsync(int id);
    Task<MotoReadDTO> AtualizarPlacaAsync(int id, MotoUpdateDTO dto);
    Task DeletarMotoAsync(int id);
}
