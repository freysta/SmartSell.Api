using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSell.Api.Models.Galdino
{
    [Table("Aluno")]
    public class Aluno
    {
        [Key]
        [Column("id_aluno")]
        public int _id { get; set; }
        
        [Column("telefone")]
        public string? _telefone { get; set; }
        
        [Column("cpf")]
        public string _cpf { get; set; } = string.Empty;
        
        [Column("endereco")]
        public string? _endereco { get; set; }
        
        [Column("cidade")]
        public string? _cidade { get; set; }
        
        [Column("curso")]
        public string? _curso { get; set; }
        
        [Column("turno")]
        public string? _turno { get; set; }
        
        [Column("fk_id_usuario")]
        public int _usuarioId { get; set; }
        
        [Column("fk_id_instituicao")]
        public int _instituicaoId { get; set; }

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
