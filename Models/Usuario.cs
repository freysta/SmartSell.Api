using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartSell.Api.Models
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        [Required]
        [MaxLength(150)]
        public string Nome { get; set; }

        [Required]
        [MaxLength(150)]
        public string Email { get; set; }

        [Required]
        [MaxLength(100)]
        public string Senha { get; set; }

        public ICollection<Cliente> Clientes { get; set; }
        public ICollection<ProdutoServico> ProdutosServicos { get; set; }
        public ICollection<Pedido> Pedidos { get; set; }
    }
}
