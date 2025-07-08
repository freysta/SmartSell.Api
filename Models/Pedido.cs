using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSell.Api.Models
{
    public class Pedido
    {
        [Key]
        public int IdPedido { get; set; }

        [Required]
        public DateTime DataPedido { get; set; }

        public string Observacoes { get; set; }

        [ForeignKey("Cliente")]
        public int FkIdCliente { get; set; }
        public Cliente Cliente { get; set; }

        [ForeignKey("Usuario")]
        public int FkIdUsuario { get; set; }
        public Usuario Usuario { get; set; }

        [ForeignKey("StatusPedido")]
        public int FkIdStatus { get; set; }
        public StatusPedido StatusPedido { get; set; }

        public ICollection<ItemPedido> ItensPedido { get; set; }
        public ICollection<Pagamento> Pagamentos { get; set; }
        public ICollection<EnderecoEntrega> EnderecosEntrega { get; set; }
        public ICollection<Agendamento> Agendamentos { get; set; }
    }
}
