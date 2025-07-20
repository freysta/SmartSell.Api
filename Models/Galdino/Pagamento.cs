using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSell.Api.Models.Galdino
{
    public enum StatusPagamentoEnum
    {
        Pago,
        Pendente,
        Atrasado
    }

    public enum FormaPagamentoEnum
    {
        PIX,
        Cartão,
        Dinheiro,
        Transferência
    }

    [Table("pagamento")]
    public class Pagamento
    {
        [Key]
        [Column("id_pagamento")]
        public int _id { get; set; }
        
        [Column("fk_id_aluno")]
        [Required]
        public int _alunoId { get; set; }
        
        [Column("valor", TypeName = "decimal(10,2)")]
        [Required]
        public decimal _valor { get; set; }
        
        [Column("data_pagamento")]
        [Required]
        public DateTime _dataPagamento { get; set; }
        
        [Column("referencia_mes")]
        [Required]
        [StringLength(7)]
        public string _referenciaMes { get; set; } = string.Empty;
        
        [Column("status")]
        [Required]
        public StatusPagamentoEnum _status { get; set; }
        
        [Column("forma_pagamento")]
        public FormaPagamentoEnum? _formaPagamento { get; set; }
        
        [Column("comprovante")]
        public byte[]? _comprovante { get; set; }

        [ForeignKey("_alunoId")]
        public virtual Aluno? Aluno { get; set; }

        public Pagamento()
        {
        }

        public Pagamento(int id, int alunoId, decimal valor, DateTime dataPagamento, string referenciaMes, StatusPagamentoEnum status)
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
