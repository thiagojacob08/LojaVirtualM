using System.ComponentModel.DataAnnotations;

namespace DesafioBackend.Application.DTO;

public class MotoCreateDTO
{
    [Required]
    public int Identificador { get; set; }

    [Required]
    [Range(1900, 2100)]
    public int Ano { get; set; }

    [Required]
    [StringLength(100)]
    public string? Modelo { get; set; }

    [Required]
    [StringLength(10)]
    public string? Placa { get; set; }
}
