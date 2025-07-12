using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSell.Api.Models.Galdino
{
    public class RotaAluno
    {
        [Key]
        public int _id { get; set; }
        
        public int _fkIdRota { get; set; }
        public int _fkIdAluno { get; set; }
        public int _fkIdPonto { get; set; }
        public string _confirmado { get; set; } = "Não";

        [ForeignKey("_fkIdRota")]
        public virtual Rota? Rota { get; set; }
        
        [ForeignKey("_fkIdAluno")]
        public virtual Aluno? Aluno { get; set; }
        
        [ForeignKey("_fkIdPonto")]
        public virtual PontoEmbarque? PontoEmbarque { get; set; }

        public RotaAluno()
        {
        }

        public RotaAluno(int fkIdRota, int fkIdAluno, int fkIdPonto, string confirmado = "Não")
        {
            _fkIdRota = fkIdRota;
            _fkIdAluno = fkIdAluno;
            _fkIdPonto = fkIdPonto;
            _confirmado = confirmado;
        }
    }
}
