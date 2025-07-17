using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSell.Api.Models.Galdino
{
    [Table("Presenca")]
    public class Presenca
    {
        [Key]
        [Column("id_presenca")]
        public int _id { get; set; }
        
        [Column("fk_id_rota")]
        public int _rotaId { get; set; }
        
        [Column("fk_id_aluno")]
        public int _alunoId { get; set; }
        
        [Column("fk_id_ponto")]
        public int _pontoId { get; set; }
        
        [Column("presente")]
        public string _presente { get; set; } = string.Empty;
        
        [Column("horario_embarque")]
        public TimeSpan? _horarioEmbarque { get; set; }
        
        [Column("horario_desembarque")]
        public TimeSpan? _horarioDesembarque { get; set; }
        
        [Column("observacao")]
        public string? _observacao { get; set; }

        public Presenca()
        {
        }

        public Presenca(int id, int rotaId, int alunoId, int pontoId, string presente)
        {
            _id = id;
            _rotaId = rotaId;
            _alunoId = alunoId;
            _pontoId = pontoId;
            _presente = presente;
        }
    }
}
