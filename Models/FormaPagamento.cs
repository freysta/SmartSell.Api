using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartSell.Api.Models
{
    public class FormaPagamento
    {
        [Key]
        public int IdFormaPagamento { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }

        [MaxLength(50)]
        public string Tipo { get; set; }

        public string AvistaParcelado { get; set; } // Enum 'A vista' or 'Parcelado'

        public string Descricao { get; set; }

        public ICollection<Pagamento> Pagamentos { get; set; }
    }
}
