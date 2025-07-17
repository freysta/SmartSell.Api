using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSell.Api.Models.Galdino
{
    [Table("GestorSistema")]
    public class GestorSistema
    {
        [Key]
        [Column("id_gestor")]
        public int _id { get; set; }
        
        [Column("nivel_acesso")]
        public int _nivelAcesso { get; set; } = 1;
        
        [Column("fk_id_usuario")]
        public int _usuarioId { get; set; }

        public GestorSistema()
        {
        }

        public GestorSistema(int id, int nivelAcesso, int usuarioId)
        {
            _id = id;
            _nivelAcesso = nivelAcesso;
            _usuarioId = usuarioId;
        }
    }
}
