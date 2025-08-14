
namespace DesafioBackend.Domain.DTO;

public class LocacaoReadDTO
{
    public int Id { get; set; }
    public int EntregadorId { get; set; }
    public int MotoId { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime DataPrevisaoTermino { get; set; }
    public DateTime? DataTerminoEfetiva { get; set; }
    public decimal ValorDiaria { get; set; }
    public decimal ValorTotal { get; set; }
    public decimal? Multa { get; set; }
    public bool Ativa { get; set; }
}
