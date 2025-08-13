using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesafioBackend.Application
{
    public class LocacaoCreateDTO
    {
        [Required]
        public int EntregadorId { get; set; }

        [Required]
        public int MotoId { get; set; }

        [Required]
        [Range(7, 50)]
        public int PlanoDias { get; set; } // só valores permitidos: 7, 15, 30, 45, 50

        [Required]
        public DateTime DataInicio { get; set; } // deve ser o dia após a criação, validar na lógica

        [Required]
        public DateTime DataTermino { get; set; }

        [Required]
        public DateTime DataPrevisaoTermino { get; set; }
    }

    public class LocacaoUpdateDevolucaoDTO
    {
        [Required]
        public DateTime DataDevolucao { get; set; }
    }
}
