using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSell.Api.Models.Galdino
{
    [Table("Presenca")]
    public class Presenca
    {
        [Key]
        [Column("id_presenca")]
        public int IdPresenca { get; set; }

        [Required]
        [Column("fk_id_rota")]
        public int FkIdRota { get; set; }

        [Required]
        [Column("fk_id_aluno")]
        public int FkIdAluno { get; set; }

        [Required]
        [Column("presente")]
        public PresencaStatus Presente { get; set; }

        [Column("observacao")]
        public string? Observacao { get; set; }

        // Relacionamentos
        [ForeignKey("FkIdRota")]
        public virtual Rota Rota { get; set; } = null!;

        [ForeignKey("FkIdAluno")]
        public virtual Aluno Aluno { get; set; } = null!;
    }

    public enum PresencaStatus
    {
        Sim,
        Não
    }
}
