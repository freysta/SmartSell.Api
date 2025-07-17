using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSell.Api.Models.Galdino
{
    public enum TipoRotaEnum
    {
        Ida,
        Volta,
        Circular
    }

    public enum StatusRotaEnum
    {
        Planejada,
        [Display(Name = "Em andamento")]
        EmAndamento,
        Concluída,
        Cancelada
    }

    [Table("Rota")]
    public class Rota
    {
        [Key]
        [Column("id_rota")]
        public int _id { get; set; }
        
        [Column("data_rota")]
        [Required]
        public DateTime _dataRota { get; set; }
        
        [Column("tipo_rota")]
        [Required]
        public TipoRotaEnum _tipoRota { get; set; }
        
        [Column("horario_saida")]
        [Required]
        public TimeSpan _horarioSaida { get; set; }
        
        [Column("horario_chegada")]
        public TimeSpan? _horarioChegada { get; set; }
        
        [Column("km_percorrido", TypeName = "decimal(8,2)")]
        public decimal? _kmPercorrido { get; set; }
        
        [Column("status")]
        public StatusRotaEnum _status { get; set; } = StatusRotaEnum.Planejada;
        
        [Column("observacoes")]
        public string? _observacoes { get; set; }
        
        [Column("fk_id_motorista")]
        [Required]
        public int _motoristaId { get; set; }
        
        [Column("fk_id_onibus")]
        [Required]
        public int _onibusId { get; set; }
        
        [Column("fk_id_instituicao")]
        [Required]
        public int _instituicaoId { get; set; }

        // Navigation Properties
        [ForeignKey("_motoristaId")]
        public virtual Motorista? Motorista { get; set; }
        
        [ForeignKey("_onibusId")]
        public virtual Onibus? Onibus { get; set; }
        
        [ForeignKey("_instituicaoId")]
        public virtual Instituicao? Instituicao { get; set; }

        public virtual ICollection<RotaPonto> RotaPontos { get; set; } = new List<RotaPonto>();
        public virtual ICollection<RotaAluno> RotaAlunos { get; set; } = new List<RotaAluno>();
        public virtual ICollection<Presenca> Presencas { get; set; } = new List<Presenca>();
        public virtual ICollection<Emergencia> Emergencias { get; set; } = new List<Emergencia>();

        public Rota()
        {
        }

        public Rota(int id, DateTime dataRota, TipoRotaEnum tipoRota, TimeSpan horarioSaida, int motoristaId, int onibusId, int instituicaoId)
        {
            _id = id;
            _dataRota = dataRota;
            _tipoRota = tipoRota;
            _horarioSaida = horarioSaida;
            _motoristaId = motoristaId;
            _onibusId = onibusId;
            _instituicaoId = instituicaoId;
        }
    }
}
