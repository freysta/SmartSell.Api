using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSell.Api.Models.Galdino
{
    [Table("rotaalunos")]
    public class RotaAluno
    {
        [Key]
        [Column("id_rota_aluno")]
        public int _id { get; set; }
        
        [Column("fk_id_rota")]
        public int _rotaId { get; set; }
        
        [Column("fk_id_aluno")]
        public int _alunoId { get; set; }
        
        [Column("fk_id_ponto")]
        public int _pontoId { get; set; }
        
        [Column("confirmado")]
        public string _confirmado { get; set; } = "Não";
        
        [Column("data_confirmacao")]
        public DateTime? _dataConfirmacao { get; set; }

        public RotaAluno()
        {
        }

        public RotaAluno(int id, int rotaId, int alunoId, int pontoId)
        {
            _id = id;
            _rotaId = rotaId;
            _alunoId = alunoId;
            _pontoId = pontoId;
        }
    }
}
