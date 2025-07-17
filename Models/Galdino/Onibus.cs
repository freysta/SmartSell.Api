using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSell.Api.Models.Galdino
{
    public enum StatusOnibusEnum
    {
        Ativo,
        Manutenção,
        Inativo
    }

    [Table("Onibus")]
    public class Onibus
    {
        [Key]
        [Column("id_onibus")]
        public int _id { get; set; }
        
        [Column("placa")]
        [Required]
        [StringLength(10)]
        public string _placa { get; set; } = string.Empty;
        
        [Column("modelo")]
        [Required]
        [StringLength(50)]
        public string _modelo { get; set; } = string.Empty;
        
        [Column("capacidade")]
        [Required]
        public int _capacidade { get; set; }
        
        [Column("ano")]
        [Required]
        public int _ano { get; set; }
        
        [Column("status")]
        public StatusOnibusEnum _status { get; set; } = StatusOnibusEnum.Ativo;

        // Navigation Properties
        public virtual ICollection<Rota> Rotas { get; set; } = new List<Rota>();

        public Onibus()
        {
        }

        public Onibus(int id, string placa, string modelo, int capacidade, int ano)
        {
            _id = id;
            _placa = placa;
            _modelo = modelo;
            _capacidade = capacidade;
            _ano = ano;
        }
    }
}
