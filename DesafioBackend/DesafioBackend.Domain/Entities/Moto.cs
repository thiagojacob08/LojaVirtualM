using System.ComponentModel.DataAnnotations;

namespace DesafioBackend.Domain.Entities;

public class Moto
{
    [Key]
    public int Identificador { get; set; }  // Id único
    public int Ano { get; set; }
    public string Modelo { get; set; } = null!;
    public string Placa { get; set; } = null!; // Deve ser único
}
