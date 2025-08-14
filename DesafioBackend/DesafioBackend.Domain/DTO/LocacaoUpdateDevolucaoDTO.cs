using System.ComponentModel.DataAnnotations;

namespace DesafioBackend.Application.DTO;

public class LocacaoUpdateDevolucaoDTO
{
    [Required]
    public DateTime DataDevolucao { get; set; }
}
