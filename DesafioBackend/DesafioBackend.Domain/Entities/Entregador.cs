using System.ComponentModel.DataAnnotations;

namespace DesafioBackend.Domain.Entities;

public class Entregador
{
    [Key]
    public int Identificador { get; set; }
    public string Nome { get; set; } = null!;
    public string CNPJ { get; set; } = null!;  // único
    public DateTime DataNascimento { get; set; }
    public string NumeroCNH { get; set; } = null!;  // único
    public string TipoCNH { get; set; } = null!;    // A, B ou A+B
    public string? ImagemCNHPath { get; set; }      // caminho para foto armazenada
}
