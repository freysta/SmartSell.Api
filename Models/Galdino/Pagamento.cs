using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSell.Api.Models.Galdino
{
    [Table("Pagamento")]
    public class Pagamento
    {
        [Key]
        [Column("id_pagamento")]
        public int IdPagamento { get; set; }

        [Required]
        [Column("fk_id_aluno")]
        public int FkIdAluno { get; set; }

        [Required]
        [Column("valor", TypeName = "decimal(10,2)")]
        public decimal Valor { get; set; }

        [Required]
        [Column("data_pagamento")]
        public DateTime DataPagamento { get; set; }

        [Required]
        [MaxLength(7)]
        [Column("referencia_mes")]
        public string ReferenciaMes { get; set; } = string.Empty; // formato: 07/2025

        [Required]
        [Column("status")]
        public StatusPagamento Status { get; set; }

        [Column("forma_pagamento")]
        public FormaPagamento? FormaPagamento { get; set; }

        [MaxLength(50)]
        [Column("metodo_pagamento")]
        public string? MetodoPagamento { get; set; }

        [Column("data_vencimento")]
        public DateTime DataVencimento { get; set; }

        // Relacionamentos
        [ForeignKey("FkIdAluno")]
        public virtual Aluno Aluno { get; set; } = null!;
    }

    public enum StatusPagamento
    {
        Pago,
        Pendente,
        Atrasado
    }

    public enum FormaPagamento
    {
        PIX,
        Cartão,
        Dinheiro,
        Transferência
    }
}
