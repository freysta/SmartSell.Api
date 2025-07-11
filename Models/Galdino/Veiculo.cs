using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSell.Api.Models.Galdino
{
    [Table("Veiculo")]
    public class Veiculo
    {
        [Key]
        [Column("id_veiculo")]
        public int IdVeiculo { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("modelo")]
        public string Modelo { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        [Column("marca")]
        public string Marca { get; set; } = string.Empty;

        [Required]
        [MaxLength(4)]
        [Column("ano")]
        public string Ano { get; set; } = string.Empty;

        [Required]
        [MaxLength(8)]
        [Column("placa")]
        public string Placa { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        [Column("cor")]
        public string Cor { get; set; } = string.Empty;

        [Required]
        [Column("capacidade")]
        public int Capacidade { get; set; }

        [MaxLength(50)]
        [Column("numero_chassi")]
        public string? NumeroChassi { get; set; }

        [MaxLength(50)]
        [Column("renavam")]
        public string? Renavam { get; set; }

        [Column("quilometragem")]
        public int? Quilometragem { get; set; }

        [Column("data_ultima_revisao")]
        public DateTime? DataUltimaRevisao { get; set; }

        [Column("data_proxima_revisao")]
        public DateTime? DataProximaRevisao { get; set; }

        [Required]
        [Column("status")]
        public StatusVeiculo Status { get; set; } = StatusVeiculo.Ativo;

        [Column("observacoes")]
        public string? Observacoes { get; set; }

        [Required]
        [Column("data_cadastro")]
        public DateTime DataCadastro { get; set; } = DateTime.Now;

        [Column("fk_id_motorista")]
        public int? FkIdMotorista { get; set; }

        // Relacionamentos
        [ForeignKey("FkIdMotorista")]
        public virtual Usuario? Motorista { get; set; }

        public virtual ICollection<Rota> Rotas { get; set; } = new List<Rota>();
    }

    public enum StatusVeiculo
    {
        Ativo,
        Inativo,
        Manutencao,
        Revisao,
        Indisponivel
    }

    public enum TipoVeiculo
    {
        Onibus,
        Van,
        Microonibus,
        Carro
    }
}
