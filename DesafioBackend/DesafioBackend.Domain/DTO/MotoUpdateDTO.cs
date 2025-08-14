using System.ComponentModel.DataAnnotations;

namespace DesafioBackend.Application.DTO;

public class MotoUpdateDTO
{
    [Required]
    [StringLength(10)]
    public string? Placa { get; set; }
}
