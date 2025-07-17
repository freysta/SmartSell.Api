using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSell.Api.Models.Galdino
{
    [Table("Pagamento")]
    public class Pagamento
    {
        [Key]
        [Column("id_pagamento")]
        public int _id { get; set; }
        
        [Column("fk_id_aluno")]
        public int _alunoId { get; set; }
        
        [Column("valor")]
        public decimal _valor { get; set; }
        
        [Column("data_pagamento")]
        public DateTime _dataPagamento { get; set; }
        
        [Column("referencia_mes")]
        public string _referenciaMes { get; set; } = string.Empty;
        
        [Column("status")]
        public string _status { get; set; } = string.Empty;
        
        [Column("forma_pagamento")]
        public string? _formaPagamento { get; set; }
        
        [Column("comprovante")]
        public byte[]? _comprovante { get; set; }

        public Pagamento()
        {
        }

        public Pagamento(int id, int alunoId, decimal valor, DateTime dataPagamento, string referenciaMes, string status)
        {
            _id = id;
            _alunoId = alunoId;
            _valor = valor;
            _dataPagamento = dataPagamento;
            _referenciaMes = referenciaMes;
            _status = status;
        }
    }
}
