using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartSell.Api.Models
{
    public class CategoriaProduto
    {
        [Key]
        public int IdCategoria { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }

        public ICollection<ProdutoServico> ProdutosServicos { get; set; }
    }
}
