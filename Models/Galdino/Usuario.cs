using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SmartSell.Api.Models.Galdino
{
    public class Usuario
    {
        [Key]
        public int _id { get; set; }
        public string _nome { get; set; } = string.Empty;
        public string _email { get; set; } = string.Empty;
        public string? _telefone { get; set; }
        
        [JsonIgnore] // Impede que a senha seja serializada no JSON
        public string _senha { get; set; } = string.Empty;
        public string _tipo { get; set; } = string.Empty;

        public Usuario()
        {
        }

        public Usuario(int id, string nome, string email, string senha, string tipo)
        {
            _id = id;
            _nome = nome;
            _email = email;
            _senha = senha;
            _tipo = tipo;
        }
    }
}
