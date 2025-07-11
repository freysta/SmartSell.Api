using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSell.Api.Models.Galdino
{
    [Table("Presencas")]
    public class Presenca
    {
        [Key]
        public int _id { get; set; }
        
        public int _routeId { get; set; }
        
        public int _studentId { get; set; }
        
        public string _status { get; set; } = string.Empty; // "present", "absent"
        
        public string? _observation { get; set; }
        
        public DateTime _date { get; set; } = DateTime.Now;
        
        public DateTime _createdAt { get; set; } = DateTime.Now;

        // Construtor padrão
        public Presenca() { }

        // Construtor com parâmetros
        public Presenca(int routeId, int studentId, string status, string? observation = null)
        {
            _routeId = routeId;
            _studentId = studentId;
            _status = status;
            _observation = observation;
            _date = DateTime.Now;
            _createdAt = DateTime.Now;
        }

        // Relacionamentos
        [ForeignKey("_routeId")]
        public virtual Rota? Rota { get; set; }

        [ForeignKey("_studentId")]
        public virtual Aluno? Aluno { get; set; }
    }
}
