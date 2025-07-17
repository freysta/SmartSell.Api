using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSell.Api.Models.Galdino
{
    [Table("Rota")]
    public class Rota
    {
        [Key]
        [Column("id_rota")]
        public int _id { get; set; }
        
        [Column("data_rota")]
        public DateTime _dataRota { get; set; }
        
        [Column("tipo_rota")]
        public string _tipoRota { get; set; } = string.Empty;
        
        [Column("horario_saida")]
        public TimeSpan _horarioSaida { get; set; }
        
        [Column("horario_chegada")]
        public TimeSpan? _horarioChegada { get; set; }
        
        [Column("km_percorrido")]
        public decimal? _kmPercorrido { get; set; }
        
        [Column("status")]
        public string _status { get; set; } = "Planejada";
        
        [Column("observacoes")]
        public string? _observacoes { get; set; }
        
        [Column("fk_id_motorista")]
        public int _motoristaId { get; set; }
        
        [Column("fk_id_onibus")]
        public int _onibusId { get; set; }
        
        [Column("fk_id_instituicao")]
        public int _instituicaoId { get; set; }

        public Rota()
        {
        }

        public Rota(int id, DateTime dataRota, string tipoRota, TimeSpan horarioSaida, int motoristaId, int onibusId, int instituicaoId)
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
