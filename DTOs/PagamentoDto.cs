using System.ComponentModel.DataAnnotations;

namespace SmartSell.Api.DTOs
{
    public class PagamentoDto
    {
        public int Id { get; set; }
        public int AlunoId { get; set; }
        public string NomeAluno { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public string Mes { get; set; } = string.Empty;
        public int Ano { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? MetodoPagamento { get; set; }
        public DateTime? DataPagamento { get; set; }
        public DateTime DataVencimento { get; set; }
        public DateTime DataCriacao { get; set; }
    }

    public class CriarPagamentoDto
    {
        [Required(ErrorMessage = "ID do aluno é obrigatório")]
        public int AlunoId { get; set; }

        [Required(ErrorMessage = "Valor é obrigatório")]
        [Range(0.01, 9999.99, ErrorMessage = "Valor deve estar entre R$ 0,01 e R$ 9.999,99")]
        public decimal Valor { get; set; }

        [Required(ErrorMessage = "Mês é obrigatório")]
        [RegularExpression(@"^\d{4}-\d{2}$", ErrorMessage = "Mês deve estar no formato YYYY-MM")]
        public string Mes { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ano é obrigatório")]
        [Range(2020, 2030, ErrorMessage = "Ano deve estar entre 2020 e 2030")]
        public int Ano { get; set; }

        [Required(ErrorMessage = "Data de vencimento é obrigatória")]
        public DateTime DataVencimento { get; set; }

        public string? MetodoPagamento { get; set; }
    }

    public class AtualizarPagamentoDto
    {
        [Range(0.01, 9999.99, ErrorMessage = "Valor deve estar entre R$ 0,01 e R$ 9.999,99")]
        public decimal? Valor { get; set; }

        [RegularExpression(@"^\d{4}-\d{2}$", ErrorMessage = "Mês deve estar no formato YYYY-MM")]
        public string? Mes { get; set; }

        [Range(2020, 2030, ErrorMessage = "Ano deve estar entre 2020 e 2030")]
        public int? Ano { get; set; }

        public DateTime? DataVencimento { get; set; }

        public string? MetodoPagamento { get; set; }

        public string? Status { get; set; }
    }

    public class MarcarPagamentoComoQuitadoDto
    {
        [Required(ErrorMessage = "Método de pagamento é obrigatório")]
        public string MetodoPagamento { get; set; } = string.Empty;

        public DateTime? DataPagamento { get; set; }
    }
}
