using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SmartSell.Api.Models.Galdino
{
    [Table("Usuario")]
    public class Usuario
    {
        [Key]
        [Column("id_usuario")]
        public int _id { get; set; }
        
        [Column("nome")]
        public string _nome { get; set; } = string.Empty;
        
        [Column("email")]
        public string _email { get; set; } = string.Empty;
        
        [JsonIgnore]
        [Column("senha")]
        public string _senha { get; set; } = string.Empty;
        
        [Column("ativo")]
        public bool _ativo { get; set; } = true;

        public Usuario()
        {
        }

        public Usuario(int id, string nome, string email, string senha)
        {
            _id = id;
            _nome = nome;
            _email = email;
            _senha = senha;
        }
    }
}
