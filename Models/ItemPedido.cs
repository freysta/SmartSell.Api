using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSell.Api.Models
{
    public class ItemPedido
    {
        [Key]
        public int IdItem { get; set; }

        [Required]
        public int Quantidade { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal PrecoUnitario { get; set; }

        [ForeignKey("Pedido")]
        public int FkIdPedido { get; set; }
        public Pedido Pedido { get; set; }

        [ForeignKey("ProdutoServico")]
        public int FkIdProduto { get; set; }
        public ProdutoServico ProdutoServico { get; set; }
    }
}
