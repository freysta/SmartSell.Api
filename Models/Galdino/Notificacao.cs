using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSell.Api.Models.Galdino
{
    [Table("Notificacao")]
    public class Notificacao
    {
        [Key]
        [Column("id_notificacao")]
        public int _id { get; set; }
        
        [Column("titulo")]
        public string? _titulo { get; set; }
        
        [Column("mensagem")]
        public string _mensagem { get; set; } = string.Empty;
        
        [Column("data_envio")]
        public DateTime _dataEnvio { get; set; } = DateTime.Now;
        
        [Column("tipo")]
        public string _tipo { get; set; } = "Informativo";
        
        [Column("lida")]
        public bool _lida { get; set; } = false;
        
        [Column("fk_id_aluno")]
        public int? _alunoId { get; set; }

        public Notificacao()
        {
        }

        public Notificacao(int id, string mensagem, string tipo)
        {
            _id = id;
            _mensagem = mensagem;
            _tipo = tipo;
        }
    }
}
