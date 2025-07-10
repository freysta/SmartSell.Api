using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSell.Api.Models.Galdino
{
    [Table("Aluno")]
    public class Aluno
    {
        [Key]
        [Column("id_aluno")]
        public int IdAluno { get; set; }

        [Required]
        [MaxLength(150)]
        [Column("nome")]
        public string Nome { get; set; } = string.Empty;

        [MaxLength(20)]
        [Column("telefone")]
        public string? Telefone { get; set; }

        [MaxLength(150)]
        [Column("email")]
        public string? Email { get; set; }

        [Required]
        [MaxLength(14)]
        [Column("cpf")]
        public string Cpf { get; set; } = string.Empty;

        // Relacionamentos
        public virtual ICollection<RotaAluno> RotaAlunos { get; set; } = new List<RotaAluno>();
        public virtual ICollection<Pagamento> Pagamentos { get; set; } = new List<Pagamento>();
        public virtual ICollection<Presenca> Presencas { get; set; } = new List<Presenca>();
        public virtual ICollection<Notificacao> Notificacoes { get; set; } = new List<Notificacao>();
    }
}
