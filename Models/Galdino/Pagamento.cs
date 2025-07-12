using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSell.Api.Models.Galdino
{
    [Table("Pagamentos")]
    public class Pagamento
    {
        [Key]
        public int _id { get; set; }
        
        public int _studentId { get; set; }
        
        public decimal _amount { get; set; }
        
        public string _month { get; set; } = string.Empty;
        
        public int _year { get; set; }
        
        public string _status { get; set; } = string.Empty; 
        
        public string? _paymentMethod { get; set; }
        
        public DateTime? _paymentDate { get; set; }
        
        public DateTime _dueDate { get; set; }
        
        public DateTime _createdAt { get; set; } = DateTime.Now;

        public Pagamento() { }

        public Pagamento(int studentId, decimal amount, string month, int year, string status, DateTime dueDate)
        {
            _studentId = studentId;
            _amount = amount;
            _month = month;
            _year = year;
            _status = status;
            _dueDate = dueDate;
            _createdAt = DateTime.Now;
        }

        [ForeignKey("_studentId")]
        public virtual Aluno? Aluno { get; set; }
    }
}
