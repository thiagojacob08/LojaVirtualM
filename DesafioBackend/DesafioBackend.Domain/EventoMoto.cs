using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesafioBackend.Domain
{
    public class EventoMoto
    {
        [Key]
        public int EventoMotoId { get; set; }
        public int MotoId { get; set; }
        public int Ano { get; set; }
        public string Modelo { get; set; } = null!;
        public string Placa { get; set; } = null!;
        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
    }
}
