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
        [Required]
        [StringLength(20)]
        public string _cnh { get; set; } = string.Empty;
        
        [Column("venc_cnh")]
        [Required]
        public DateTime _vencCnh { get; set; }
        
        [Column("cpf")]
        [Required]
        [StringLength(14)]
        public string _cpf { get; set; } = string.Empty;
        
        [Column("data_nascimento")]
        [Required]
        public DateTime _dataNascimento { get; set; }
        
        [Column("telefone")]
        [StringLength(20)]
        public string? _telefone { get; set; }
        
        [Column("fk_id_usuario")]
        [Required]
        public int _usuarioId { get; set; }

        [ForeignKey("_usuarioId")]
        public virtual Usuario? Usuario { get; set; }

        public virtual ICollection<Rota> Rotas { get; set; } = new List<Rota>();

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
