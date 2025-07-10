using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSell.Api.Models.Galdino
{
    [Table("PontoEmbarque")]
    public class PontoEmbarque
    {
        [Key]
        [Column("id_ponto_embarque")]
        public int IdPontoEmbarque { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("nome")]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [MaxLength(300)]
        [Column("endereco")]
        public string Endereco { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Column("bairro")]
        public string Bairro { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Column("cidade")]
        public string Cidade { get; set; } = string.Empty;

        [Column("latitude")]
        public double? Latitude { get; set; }

        [Column("longitude")]
        public double? Longitude { get; set; }

        [Required]
        [Column("status")]
        public StatusPontoEmbarque Status { get; set; } = StatusPontoEmbarque.Ativo;

        [Column("rotas_ids")]
        public string RotasIds { get; set; } = "[]"; // JSON array de IDs das rotas

        [MaxLength(150)]
        [Column("ponto_referencia")]
        public string? PontoReferencia { get; set; }

        // Relacionamentos
        public virtual ICollection<RotaAluno> RotaAlunos { get; set; } = new List<RotaAluno>();
    }

    public enum StatusPontoEmbarque
    {
        Ativo,
        Inativo,
        Manutencao
    }
}
