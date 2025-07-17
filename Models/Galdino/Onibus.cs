using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSell.Api.Models.Galdino
{
    [Table("Onibus")]
    public class Onibus
    {
        [Key]
        [Column("id_onibus")]
        public int _id { get; set; }
        
        [Column("placa")]
        public string _placa { get; set; } = string.Empty;
        
        [Column("modelo")]
        public string _modelo { get; set; } = string.Empty;
        
        [Column("capacidade")]
        public int _capacidade { get; set; }
        
        [Column("ano")]
        public int _ano { get; set; }
        
        [Column("status")]
        public string _status { get; set; } = "Ativo";

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
