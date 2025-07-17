using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSell.Api.Models.Galdino
{
    [Table("PontoEmbarque")]
    public class PontoEmbarque
    {
        [Key]
        [Column("id_ponto")]
        public int _id { get; set; }
        
        [Column("nome")]
        public string _nome { get; set; } = string.Empty;
        
        [Column("rua")]
        public string? _rua { get; set; }
        
        [Column("bairro")]
        public string? _bairro { get; set; }
        
        [Column("cidade")]
        public string? _cidade { get; set; }
        
        [Column("ponto_referencia")]
        public string? _pontoReferencia { get; set; }
        
        [Column("tipo_ponto")]
        public string _tipoPonto { get; set; } = "Ambos";
        
        [Column("horario_previsto")]
        public TimeSpan? _horarioPrevisto { get; set; }
        
        [Column("latitude")]
        public decimal? _latitude { get; set; }
        
        [Column("longitude")]
        public decimal? _longitude { get; set; }
        
        [Column("ordem_ida")]
        public int? _ordemIda { get; set; }
        
        [Column("ordem_volta")]
        public int? _ordemVolta { get; set; }

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
