using System.ComponentModel.DataAnnotations;

namespace SmartSell.Api.Models.Galdino
{
    public class Rota
    {
        [Key]
        public int _id { get; set; }
        public DateTime _dataRota { get; set; }
        public string _destino { get; set; } = string.Empty;
        public TimeSpan _horarioSaida { get; set; }
        public string _status { get; set; } = string.Empty;
        public int _fkIdMotorista { get; set; }

        public Rota()
        {
        }

        public Rota(int id, DateTime dataRota, string destino, TimeSpan horarioSaida, string status, int fkIdMotorista)
        {
            _id = id;
            _dataRota = dataRota;
            _destino = destino;
            _horarioSaida = horarioSaida;
            _status = status;
            _fkIdMotorista = fkIdMotorista;
        }
    }
}
