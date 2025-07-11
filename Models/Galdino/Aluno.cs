using System.ComponentModel.DataAnnotations;

namespace SmartSell.Api.Models.Galdino
{
    public class Aluno
    {
        [Key]
        public int _id { get; set; }
        public string _nome { get; set; } = string.Empty;
        public string? _telefone { get; set; }
        public string? _email { get; set; }
        public string _cpf { get; set; } = string.Empty;

        public Aluno()
        {
        }

        public Aluno(int id, string nome, string cpf)
        {
            _id = id;
            _nome = nome;
            _cpf = cpf;
        }
    }
}
