using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSell.Api.Models.Galdino
{
    [Table("RotaPonto")]
    public class RotaPonto
    {
        [Key]
        [Column("id_rota_ponto")]
        public int _id { get; set; }
        
        [Column("fk_id_rota")]
        public int _rotaId { get; set; }
        
        [Column("fk_id_ponto")]
        public int _pontoId { get; set; }
        
        [Column("ordem")]
        public int _ordem { get; set; }
        
        [Column("horario_previsto")]
        public TimeSpan? _horarioPrevisto { get; set; }
        
        [Column("horario_real")]
        public TimeSpan? _horarioReal { get; set; }

        public RotaPonto()
        {
        }

        public RotaPonto(int id, int rotaId, int pontoId, int ordem)
        {
            _id = id;
            _rotaId = rotaId;
            _pontoId = pontoId;
            _ordem = ordem;
        }
    }
}
