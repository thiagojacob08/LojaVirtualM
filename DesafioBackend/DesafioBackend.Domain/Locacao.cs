using System.ComponentModel.DataAnnotations;

namespace DesafioBackend.Domain;

public class Locacao
{
    [Key]
    public int Id { get; set; }
    public int MotoId { get; set; }
    public Moto Moto { get; set; } = null!;
    public int EntregadorId { get; set; }
    public Entregador Entregador { get; set; } = null!;
    public DateTime DataInicio { get; set; }
    public DateTime DataFimPrevisto { get; set; }
    public DateTime? DataFimReal { get; set; }

    public int PlanoDias { get; set; }  // 7, 15, 30, 45 ou 50
}
