using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSell.Api.Models.Galdino
{
    public enum TurnoEnum
    {
        Matutino,
        Vespertino,
        Noturno,
        Integral
    }

    [Table("Aluno")]
    public class Aluno
    {
        [Key]
        [Column("id_aluno")]
        public int _id { get; set; }
        
        [Column("telefone")]
        [StringLength(20)]
        public string? _telefone { get; set; }
        
        [Column("cpf")]
        [Required]
        [StringLength(14)]
        public string _cpf { get; set; } = string.Empty;
        
        [Column("endereco")]
        [StringLength(200)]
        public string? _endereco { get; set; }
        
        [Column("cidade")]
        [StringLength(100)]
        public string? _cidade { get; set; }
        
        [Column("curso")]
        [StringLength(100)]
        public string? _curso { get; set; }
        
        [Column("turno")]
        public TurnoEnum? _turno { get; set; }
        
        [Column("fk_id_usuario")]
        [Required]
        public int _usuarioId { get; set; }
        
        [Column("fk_id_instituicao")]
        [Required]
        public int _instituicaoId { get; set; }

        [ForeignKey("_usuarioId")]
        public virtual Usuario? Usuario { get; set; }
        
        [ForeignKey("_instituicaoId")]
        public virtual Instituicao? Instituicao { get; set; }

        public virtual ICollection<RotaAluno> RotaAlunos { get; set; } = new List<RotaAluno>();
        public virtual ICollection<Presenca> Presencas { get; set; } = new List<Presenca>();
        public virtual ICollection<Pagamento> Pagamentos { get; set; } = new List<Pagamento>();
        public virtual ICollection<Notificacao> Notificacoes { get; set; } = new List<Notificacao>();

        public Aluno()
        {
        }

        public Aluno(int id, string cpf, int usuarioId, int instituicaoId)
        {
            _id = id;
            _cpf = cpf;
            _usuarioId = usuarioId;
            _instituicaoId = instituicaoId;
        }
    }
}
