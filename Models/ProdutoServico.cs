using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSell.Api.Models
{
    public class ProdutoServico
    {
        [Key]
        public int IdProduto { get; set; }

        [Required]
        [MaxLength(150)]
        public string Nome { get; set; }

        public string Descricao { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Preco { get; set; }

        [Required]
        public string Tipo { get; set; } // Could be enum 'Produto' or 'Serviço'

        [ForeignKey("Usuario")]
        public int FkIdUsuario { get; set; }
        public Usuario Usuario { get; set; }

        [ForeignKey("CategoriaProduto")]
        public int FkIdCategoria { get; set; }
        public CategoriaProduto CategoriaProduto { get; set; }
    }
}
