using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSell.Api.Models.Galdino
{
    [Table("Notificacoes")]
    public class Notificacao
    {
        [Key]
        public int _id { get; set; }
        
        public string _title { get; set; } = string.Empty;
        
        public string _message { get; set; } = string.Empty;
        
        public string _type { get; set; } = string.Empty;
        
        public string _priority { get; set; } = string.Empty;
        
        public string _targetType { get; set; } = string.Empty;
        
        public string? _targetIds { get; set; }
        
        public DateTime _createdAt { get; set; } = DateTime.Now;
        
        public string? _readBy { get; set; }

        public Notificacao() { }

        public Notificacao(string title, string message, string type, string priority, string targetType)
        {
            _title = title;
            _message = message;
            _type = type;
            _priority = priority;
            _targetType = targetType;
            _createdAt = DateTime.Now;
            _readBy = "[]";
        }
    }
}
