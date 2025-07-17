using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSell.Api.Models.Galdino
{
    public enum TipoPontoEnum
    {
        Embarque,
        Desembarque,
        Ambos
    }

    [Table("PontoEmbarque")]
    public class PontoEmbarque
    {
        [Key]
        [Column("id_ponto")]
        public int _id { get; set; }
        
        [Column("nome")]
        [Required]
        [StringLength(100)]
        public string _nome { get; set; } = string.Empty;
        
        [Column("rua")]
        [StringLength(100)]
        public string? _rua { get; set; }
        
        [Column("bairro")]
        [StringLength(100)]
        public string? _bairro { get; set; }
        
        [Column("cidade")]
        [StringLength(100)]
        public string? _cidade { get; set; }
        
        [Column("ponto_referencia")]
        [StringLength(150)]
        public string? _pontoReferencia { get; set; }
        
        [Column("tipo_ponto")]
        public TipoPontoEnum _tipoPonto { get; set; } = TipoPontoEnum.Ambos;
        
        [Column("horario_previsto")]
        public TimeSpan? _horarioPrevisto { get; set; }
        
        [Column("latitude", TypeName = "decimal(10,8)")]
        public decimal? _latitude { get; set; }
        
        [Column("longitude", TypeName = "decimal(11,8)")]
        public decimal? _longitude { get; set; }
        
        [Column("ordem_ida")]
        public int? _ordemIda { get; set; }
        
        [Column("ordem_volta")]
        public int? _ordemVolta { get; set; }

        public virtual ICollection<RotaPonto> RotaPontos { get; set; } = new List<RotaPonto>();
        public virtual ICollection<RotaAluno> RotaAlunos { get; set; } = new List<RotaAluno>();
        public virtual ICollection<Presenca> Presencas { get; set; } = new List<Presenca>();

        public PontoEmbarque()
        {
        }

        public PontoEmbarque(int id, string nome)
        {
            _id = id;
            _nome = nome;
        }
    }
}
