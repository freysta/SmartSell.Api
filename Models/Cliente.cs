using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSell.Api.Models
{
    public class Cliente
    {
        [Key]
        public int IdCliente { get; set; }

        [Required]
        [MaxLength(150)]
        public string Nome { get; set; }

        [MaxLength(50)]
        public string Telefone { get; set; }

        [MaxLength(150)]
        public string Email { get; set; }

        [MaxLength(14)]
        public string Cpf { get; set; }

        [ForeignKey("Usuario")]
        public int? FkIdUsuario { get; set; }
        public Usuario Usuario { get; set; }
    }
}
