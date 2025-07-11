using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSell.Api.Models.Galdino
{
    [Table("Usuario")]
    public class Usuario
    {
        [Key]
        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Required]
        [MaxLength(150)]
        [Column("nome")]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        [Column("telefone")]
        public string? Telefone { get; set; }

        [Required]
        [Column("senha")]
        public string Senha { get; set; } = string.Empty;

        [Required]
        [Column("tipo")]
        public TipoUsuario Tipo { get; set; }
    }

    public enum TipoUsuario
    {
        Admin,
        Motorista,
        Aluno
    }
}
