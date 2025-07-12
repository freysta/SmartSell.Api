namespace SmartSell.Api.DTOs
{
    public class CreateRotaAlunoDto
    {
        public int FkIdRota { get; set; }
        public int FkIdAluno { get; set; }
        public int FkIdPonto { get; set; }
        public string Confirmado { get; set; } = "Não";
    }

    public class UpdateRotaAlunoDto
    {
        public int FkIdRota { get; set; }
        public int FkIdAluno { get; set; }
        public int FkIdPonto { get; set; }
        public string Confirmado { get; set; } = "Não";
    }

    public class RotaAlunoResponseDto
    {
        public int Id { get; set; }
        public int FkIdRota { get; set; }
        public int FkIdAluno { get; set; }
        public int FkIdPonto { get; set; }
        public string Confirmado { get; set; } = string.Empty;
        public string? NomeAluno { get; set; }
        public string? DestinoRota { get; set; }
        public string? NomePonto { get; set; }
    }
}
