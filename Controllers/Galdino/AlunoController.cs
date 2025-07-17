using Microsoft.AspNetCore.Mvc;
using SmartSell.Api.DAO;
using SmartSell.Api.Data;
using SmartSell.Api.Models.Galdino;

namespace SmartSell.Api.Controllers.Galdino
{
    [ApiController]
    [Route("aluno")]
    public class AlunoController : ControllerBase
    {
        private readonly AlunoDAO _alunoDAO;

        public AlunoController(GaldinoDbContext context)
        {
            _alunoDAO = new AlunoDAO(context);
        }

        [HttpGet]
        public IActionResult GetAll(string nome = "")
        {
            try
            {
                var alunos = _alunoDAO.GetAll(nome);
                return Ok(alunos);
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
                var aluno = _alunoDAO.GetById(id);
                if (aluno == null)
                    return NotFound("Aluno não encontrado");
                
                return Ok(aluno);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] Aluno aluno)
        {
            try
            {
                var novoAluno = _alunoDAO.Create(aluno);
                return Ok(novoAluno);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Aluno aluno)
        {
            try
            {
                aluno._id = id;
                var alunoAtualizado = _alunoDAO.Update(aluno);
                return Ok(alunoAtualizado);
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
                var sucesso = _alunoDAO.Delete(id);
                if (sucesso)
                    return Ok("Aluno removido com sucesso");
                else
                    return NotFound("Aluno não encontrado");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("cpf/{cpf}")]
        public IActionResult GetByCpf(string cpf)
        {
            try
            {
                var aluno = _alunoDAO.GetByCpf(cpf);
                if (aluno == null)
                    return NotFound("Aluno não encontrado");
                
                return Ok(aluno);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
