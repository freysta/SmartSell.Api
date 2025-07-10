using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSell.Api.Models.Galdino
{
    [Table("RotaAluno")]
    public class RotaAluno
    {
        [Key]
        [Column("id_rota_aluno")]
        public int IdRotaAluno { get; set; }

        [Required]
        [Column("fk_id_rota")]
        public int FkIdRota { get; set; }

        [Required]
        [Column("fk_id_aluno")]
        public int FkIdAluno { get; set; }

        [Required]
        [Column("fk_id_ponto")]
        public int FkIdPonto { get; set; }

        [Column("confirmado")]
        public ConfirmacaoStatus Confirmado { get; set; } = ConfirmacaoStatus.Não;

        // Relacionamentos
        [ForeignKey("FkIdRota")]
        public virtual Rota Rota { get; set; } = null!;

        [ForeignKey("FkIdAluno")]
        public virtual Aluno Aluno { get; set; } = null!;

        [ForeignKey("FkIdPonto")]
        public virtual PontoEmbarque PontoEmbarque { get; set; } = null!;
    }

    public enum ConfirmacaoStatus
    {
        Sim,
        Não
    }
}
