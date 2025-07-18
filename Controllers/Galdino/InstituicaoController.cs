using Microsoft.AspNetCore.Mvc;
using SmartSell.Api.DAO;
using SmartSell.Api.Data;
using SmartSell.Api.Models.Galdino;

namespace SmartSell.Api.Controllers.Galdino
{
    [ApiController]
    [Route("api/instituicoes")]
    public class InstituicaoController : ControllerBase
    {
        private readonly InstituicaoDAO _instituicaoDAO;

        public InstituicaoController(GaldinoDbContext context)
        {
            _instituicaoDAO = new InstituicaoDAO(context);
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] string? nome = null)
        {
            try
            {
                var instituicoes = _instituicaoDAO.GetAll(nome ?? "")
                    .Select(i => new
                    {
                        id = i._id,
                        nome = i._nome,
                        cidade = i._cidade,
                        endereco = i._endereco,
                        telefone = i._telefone,
                        cep = i._cep,
                        createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
                    }).ToList();

                return Ok(new { data = instituicoes, message = "Instituições listadas com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var instituicao = _instituicaoDAO.GetById(id);

                if (instituicao == null)
                    return NotFound("Instituição não encontrada");

                var response = new
                {
                    id = instituicao._id,
                    nome = instituicao._nome,
                    cidade = instituicao._cidade,
                    endereco = instituicao._endereco,
                    telefone = instituicao._telefone,
                    cep = instituicao._cep,
                    createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
                };

                return Ok(new { data = response, message = "Instituição encontrada" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateInstituicaoRequest request)
        {
            try
            {
                // Validar campos obrigatórios
                if (string.IsNullOrEmpty(request.Nome) || string.IsNullOrEmpty(request.Cidade))
                {
                    return BadRequest(new { message = "Nome e cidade são obrigatórios" });
                }

                var instituicao = new Instituicao
                {
                    _nome = request.Nome,
                    _cidade = request.Cidade,
                    _endereco = request.Endereco,
                    _telefone = request.Telefone,
                    _cep = request.Cep
                };

                _instituicaoDAO.Create(instituicao);

                var response = new
                {
                    id = instituicao._id,
                    nome = instituicao._nome,
                    cidade = instituicao._cidade,
                    endereco = instituicao._endereco,
                    telefone = instituicao._telefone,
                    cep = instituicao._cep,
                    createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
                };

                return StatusCode(201, new { data = response, message = "Instituição criada com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateInstituicaoRequest request)
        {
            try
            {
                var instituicao = _instituicaoDAO.GetById(id);

                if (instituicao == null)
                    return NotFound("Instituição não encontrada");

                instituicao._nome = request.Nome ?? instituicao._nome;
                instituicao._cidade = request.Cidade ?? instituicao._cidade;
                instituicao._endereco = request.Endereco ?? instituicao._endereco;
                instituicao._telefone = request.Telefone ?? instituicao._telefone;
                instituicao._cep = request.Cep ?? instituicao._cep;

                _instituicaoDAO.Update(instituicao);

                var response = new
                {
                    id = instituicao._id,
                    nome = instituicao._nome,
                    cidade = instituicao._cidade,
                    endereco = instituicao._endereco,
                    telefone = instituicao._telefone,
                    cep = instituicao._cep,
                    createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
                };

                return Ok(new { data = response, message = "Instituição atualizada com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var instituicao = _instituicaoDAO.GetById(id);

                if (instituicao == null)
                    return NotFound("Instituição não encontrada");

                _instituicaoDAO.Delete(id);
                return Ok(new { message = "Instituição removida com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }

    public class CreateInstituicaoRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string? Endereco { get; set; }
        public string? Telefone { get; set; }
        public string? Cep { get; set; }
    }

    public class UpdateInstituicaoRequest
    {
        public string? Nome { get; set; }
        public string? Cidade { get; set; }
        public string? Endereco { get; set; }
        public string? Telefone { get; set; }
        public string? Cep { get; set; }
    }
}
