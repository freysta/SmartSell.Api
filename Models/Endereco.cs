using System.ComponentModel.DataAnnotations;

namespace SmartSell.Api.Models
{
    public class Endereco
    {
        [Key]
        public int IdEndereco { get; set; }

        [MaxLength(150)]
        public string Rua { get; set; }

        [MaxLength(20)]
        public string Numero { get; set; }

        [MaxLength(100)]
        public string Bairro { get; set; }

        [MaxLength(100)]
        public string Cidade { get; set; }

        [MaxLength(50)]
        public string Estado { get; set; }

        [MaxLength(100)]
        public string Complemento { get; set; }
    }
}
