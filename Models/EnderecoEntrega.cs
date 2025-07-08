using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSell.Api.Models
{
    public class EnderecoEntrega
    {
        [Key]
        public int IdEnderecoEntrega { get; set; }

        [ForeignKey("Pedido")]
        public int FkIdPedido { get; set; }
        public Pedido Pedido { get; set; }

        [ForeignKey("Endereco")]
        public int FkIdEndereco { get; set; }
        public Endereco Endereco { get; set; }
    }
}
