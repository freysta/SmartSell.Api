using System.ComponentModel.DataAnnotations;

namespace SmartSell.Api.DTOs
{
    public class NotificacaoDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Mensagem { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Prioridade { get; set; } = string.Empty;
        public string TipoDestino { get; set; } = string.Empty;
        public List<int>? IdsDestino { get; set; }
        public DateTime DataCriacao { get; set; }
        public List<int> LidaPor { get; set; } = new List<int>();
        public bool Lida { get; set; }
    }

    public class CriarNotificacaoDto
    {
        [Required(ErrorMessage = "Título é obrigatório")]
        [MaxLength(200, ErrorMessage = "Título deve ter no máximo 200 caracteres")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mensagem é obrigatória")]
        [MaxLength(1000, ErrorMessage = "Mensagem deve ter no máximo 1000 caracteres")]
        public string Mensagem { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tipo é obrigatório")]
        public string Tipo { get; set; } = string.Empty; // info, warning, success, error

        [Required(ErrorMessage = "Prioridade é obrigatória")]
        public string Prioridade { get; set; } = string.Empty; // low, normal, high

        [Required(ErrorMessage = "Tipo de destino é obrigatório")]
        public string TipoDestino { get; set; } = string.Empty; // all, students, drivers, specific

        public List<int>? IdsDestino { get; set; }
    }

    public class MarcarNotificacaoComoLidaDto
    {
        [Required(ErrorMessage = "ID do usuário é obrigatório")]
        public int UsuarioId { get; set; }
    }

    public class FiltroNotificacaoDto
    {
        public int? UsuarioId { get; set; }
        public string? Tipo { get; set; }
        public string? Prioridade { get; set; }
        public bool? ApenasNaoLidas { get; set; }
        public int Pagina { get; set; } = 1;
        public int TamanhoPagina { get; set; } = 10;
    }
}
