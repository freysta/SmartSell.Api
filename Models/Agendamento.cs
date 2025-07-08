using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSell.Api.Models
{
    public class Agendamento
    {
        [Key]
        public int IdAgendamento { get; set; }

        [Required]
        public DateTime DataAgendada { get; set; }

        [Required]
        public TimeSpan HoraAgendada { get; set; }

        public string Observacoes { get; set; }

        [ForeignKey("Pedido")]
        public int FkIdPedido { get; set; }
        public Pedido Pedido { get; set; }
    }
}
