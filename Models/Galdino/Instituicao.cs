using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSell.Api.Models.Galdino
{
    [Table("Instituicao")]
    public class Instituicao
    {
        [Key]
        [Column("id_instituicao")]
        public int _id { get; set; }
        
        [Column("nome")]
        public string _nome { get; set; } = string.Empty;
        
        [Column("cidade")]
        public string _cidade { get; set; } = string.Empty;
        
        [Column("endereco")]
        public string? _endereco { get; set; }
        
        [Column("telefone")]
        public string? _telefone { get; set; }
        
        [Column("cep")]
        public string? _cep { get; set; }

        public Instituicao()
        {
        }

        public Instituicao(int id, string nome, string cidade)
        {
            _id = id;
            _nome = nome;
            _cidade = cidade;
        }
    }
}
