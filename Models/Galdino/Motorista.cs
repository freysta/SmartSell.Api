using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSell.Api.Models.Galdino
{
    [Table("Motorista")]
    public class Motorista
    {
        [Key]
        [Column("id_motorista")]
        public int _id { get; set; }
        
        [Column("cnh")]
        public string _cnh { get; set; } = string.Empty;
        
        [Column("venc_cnh")]
        public DateTime _vencCnh { get; set; }
        
        [Column("cpf")]
        public string _cpf { get; set; } = string.Empty;
        
        [Column("data_nascimento")]
        public DateTime _dataNascimento { get; set; }
        
        [Column("telefone")]
        public string? _telefone { get; set; }
        
        [Column("fk_id_usuario")]
        public int _usuarioId { get; set; }

        public Motorista()
        {
        }

        public Motorista(int id, string cnh, string cpf, DateTime dataNascimento, int usuarioId)
        {
            _id = id;
            _cnh = cnh;
            _cpf = cpf;
            _dataNascimento = dataNascimento;
            _usuarioId = usuarioId;
        }
    }
}
