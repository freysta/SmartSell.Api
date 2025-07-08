using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSell.Api.Models
{
    public class Pagamento
    {
        [Key]
        public int IdPagamento { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal ValorPago { get; set; }

        [Required]
        public DateTime DataPagamento { get; set; }

        [ForeignKey("Pedido")]
        public int FkIdPedido { get; set; }
        public Pedido Pedido { get; set; }

        [ForeignKey("FormaPagamento")]
        public int FkIdFormaPagamento { get; set; }
        public FormaPagamento FormaPagamento { get; set; }
    }
}
