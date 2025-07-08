using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartSell.Api.Models
{
    public class StatusPedido
    {
        [Key]
        public int IdStatus { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nome { get; set; }

        public ICollection<Pedido> Pedidos { get; set; }
    }
}
