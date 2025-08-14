
namespace DesafioBackend.Domain.DTO;

public class EntregadorReadDTO
{
    public int Identificador { get; set; }
    public string Nome { get; set; }
    public string CNPJ { get; set; }
    public DateTime DataNascimento { get; set; }
    public string NumeroCNH { get; set; }
    public string TipoCNH { get; set; }
    public string? FotoCNHPath { get; set; }
}
