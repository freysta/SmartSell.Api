using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSell.Api.Models.Galdino
{
    [Table("Rota")]
    public class Rota
    {
        [Key]
        [Column("id_rota")]
        public int IdRota { get; set; }

        [Required]
        [Column("data_rota")]
        public DateTime DataRota { get; set; }

        [Required]
        [MaxLength(150)]
        [Column("destino")]
        public string Destino { get; set; } = string.Empty;

        [Required]
        [Column("horario_saida")]
        public TimeSpan HorarioSaida { get; set; }

        [Required]
        [Column("status")]
        public StatusRota Status { get; set; }

        [Required]
        [Column("fk_id_motorista")]
        public int FkIdMotorista { get; set; }
    }

    public enum StatusRota
    {
        Planejada,
        EmAndamento,
        Concluida,
        Cancelada
    }
}
