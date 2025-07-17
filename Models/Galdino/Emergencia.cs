using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSell.Api.Models.Galdino
{
    [Table("Emergencia")]
    public class Emergencia
    {
        [Key]
        [Column("id_emergencia")]
        public int _id { get; set; }
        
        [Column("fk_id_rota")]
        public int _rotaId { get; set; }
        
        [Column("tipo_emergencia")]
        public string _tipoEmergencia { get; set; } = string.Empty;
        
        [Column("descricao")]
        public string _descricao { get; set; } = string.Empty;
        
        [Column("data_hora")]
        public DateTime _dataHora { get; set; } = DateTime.Now;
        
        [Column("resolvido")]
        public bool _resolvido { get; set; } = false;
        
        [Column("observacoes")]
        public string? _observacoes { get; set; }

        public Emergencia()
        {
        }

        public Emergencia(int id, int rotaId, string tipoEmergencia, string descricao)
        {
            _id = id;
            _rotaId = rotaId;
            _tipoEmergencia = tipoEmergencia;
            _descricao = descricao;
        }
    }
}
