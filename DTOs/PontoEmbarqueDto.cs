using System.ComponentModel.DataAnnotations;

namespace SmartSell.Api.DTOs
{
    public class PontoEmbarqueDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public CoordenadaDto? Coordenadas { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<int> Rotas { get; set; } = new List<int>();
        public DateTime DataCriacao { get; set; }
    }

    public class CoordenadaDto
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class CriarPontoEmbarqueDto
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [MaxLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Endereço é obrigatório")]
        [MaxLength(300, ErrorMessage = "Endereço deve ter no máximo 300 caracteres")]
        public string Endereco { get; set; } = string.Empty;

        [Required(ErrorMessage = "Bairro é obrigatório")]
        [MaxLength(100, ErrorMessage = "Bairro deve ter no máximo 100 caracteres")]
        public string Bairro { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cidade é obrigatória")]
        [MaxLength(100, ErrorMessage = "Cidade deve ter no máximo 100 caracteres")]
        public string Cidade { get; set; } = string.Empty;

        [Range(-90, 90, ErrorMessage = "Latitude deve estar entre -90 e 90")]
        public double? Latitude { get; set; }

        [Range(-180, 180, ErrorMessage = "Longitude deve estar entre -180 e 180")]
        public double? Longitude { get; set; }

        public List<int>? RotasIds { get; set; }
    }

    public class AtualizarPontoEmbarqueDto
    {
        [MaxLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
        public string? Nome { get; set; }

        [MaxLength(300, ErrorMessage = "Endereço deve ter no máximo 300 caracteres")]
        public string? Endereco { get; set; }

        [MaxLength(100, ErrorMessage = "Bairro deve ter no máximo 100 caracteres")]
        public string? Bairro { get; set; }

        [MaxLength(100, ErrorMessage = "Cidade deve ter no máximo 100 caracteres")]
        public string? Cidade { get; set; }

        [Range(-90, 90, ErrorMessage = "Latitude deve estar entre -90 e 90")]
        public double? Latitude { get; set; }

        [Range(-180, 180, ErrorMessage = "Longitude deve estar entre -180 e 180")]
        public double? Longitude { get; set; }

        public string? Status { get; set; }

        public List<int>? RotasIds { get; set; }
    }

    public class FiltroPontoEmbarqueDto
    {
        public string? Cidade { get; set; }
        public string? Bairro { get; set; }
        public string? Status { get; set; }
        public int? RotaId { get; set; }
        public int Pagina { get; set; } = 1;
        public int TamanhoPagina { get; set; } = 10;
    }
}
