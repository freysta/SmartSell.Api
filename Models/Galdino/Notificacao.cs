using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSell.Api.Models.Galdino
{
    [Table("Notificacao")]
    public class Notificacao
    {
        [Key]
        [Column("id_notificacao")]
        public int IdNotificacao { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("titulo")]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        [Column("mensagem")]
        public string Mensagem { get; set; } = string.Empty;

        [Required]
        [Column("tipo")]
        public TipoNotificacao Tipo { get; set; }

        [Required]
        [Column("prioridade")]
        public PrioridadeNotificacao Prioridade { get; set; }

        [Required]
        [Column("tipo_destino")]
        public TipoDestinoNotificacao TipoDestino { get; set; }

        [Column("ids_destino")]
        public string? IdsDestino { get; set; } // JSON array de IDs

        [Required]
        [Column("data_criacao")]
        public DateTime DataCriacao { get; set; } = DateTime.Now;

        [Column("lida_por")]
        public string LidaPor { get; set; } = "[]"; // JSON array de IDs de usuários que leram

        [Column("fk_id_aluno")]
        public int? FkIdAluno { get; set; }

        // Relacionamentos
        [ForeignKey("FkIdAluno")]
        public virtual Aluno? Aluno { get; set; }
    }

    public enum TipoNotificacao
    {
        Info,
        Warning,
        Success,
        Error
    }

    public enum PrioridadeNotificacao
    {
        Low,
        Normal,
        High
    }

    public enum TipoDestinoNotificacao
    {
        Todos,
        Estudantes,
        Motoristas,
        Especifico
    }
}
