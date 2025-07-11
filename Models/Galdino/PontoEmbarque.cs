using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSell.Api.Models.Galdino
{
    [Table("PontosEmbarque")]
    public class PontoEmbarque
    {
        [Key]
        public int _id { get; set; }
        
        public string _name { get; set; } = string.Empty;
        
        public string _address { get; set; } = string.Empty;
        
        public string _neighborhood { get; set; } = string.Empty;
        
        public string _city { get; set; } = string.Empty;
        
        public double? _lat { get; set; }
        
        public double? _lng { get; set; }
        
        public string _status { get; set; } = string.Empty; // "active", "inactive", "maintenance"
        
        public string? _routes { get; set; } // JSON array como string
        
        public DateTime _createdAt { get; set; } = DateTime.Now;

        // Construtor padrão
        public PontoEmbarque() { }

        // Construtor com parâmetros
        public PontoEmbarque(string name, string address, string neighborhood, string city, string status)
        {
            _name = name;
            _address = address;
            _neighborhood = neighborhood;
            _city = city;
            _status = status;
            _createdAt = DateTime.Now;
            _routes = "[]";
        }
    }
}
