using Microsoft.AspNetCore.Mvc;
using SmartSell.Api.DAO;
using SmartSell.Api.Data;
using SmartSell.Api.Models.Galdino;

namespace SmartSell.Api.Controllers.Galdino
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly AlunoDAO _alunoDAO;

        public StudentsController(GaldinoDbContext context)
        {
            _alunoDAO = new AlunoDAO(context);
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] string? status, [FromQuery] string? route)
        {
            try
            {
                var alunos = _alunoDAO.GetAll().Select(a => new
                {
                    id = a._id,
                    name = "",
                    email = "",
                    phone = a._telefone,
                    cpf = a._cpf,
                    paymentStatus = (string?)null,
                    route = (string?)null,
                    enrollmentDate = (string?)null,
                    status = "active",
                    createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
                }).ToList();

                return Ok(new { data = alunos, message = "Alunos listados com sucesso" });
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

                var response = new
                {
                    id = aluno._id,
                    name = "",
                    email = "",
                    phone = aluno._telefone,
                    cpf = aluno._cpf,
                    paymentStatus = (string?)null,
                    route = (string?)null,
                    enrollmentDate = (string?)null,
                    status = "active",
                    createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
                };

                return Ok(new { data = response, message = "Aluno encontrado" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateStudentRequest request)
        {
            try
            {
                // Remover verificação de email duplicado pois Aluno não tem mais email

                var existingCpf = _alunoDAO.GetByCpf(request.Cpf);

                if (existingCpf != null)
                {
                    return BadRequest(new { message = "CPF já está em uso" });
                }

                var aluno = new Aluno
                {
                    _telefone = request.Phone,
                    _cpf = request.Cpf,
                    _usuarioId = 1, // Valor padrão
                    _instituicaoId = 1 // Valor padrão
                };

                _alunoDAO.Create(aluno);

                var response = new
                {
                    id = aluno._id,
                    name = request.Name,
                    email = request.Email,
                    phone = aluno._telefone,
                    cpf = aluno._cpf,
                    paymentStatus = (string?)null,
                    route = request.Route,
                    enrollmentDate = request.EnrollmentDate,
                    status = "active",
                    createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
                };

                return StatusCode(201, new { data = response, message = "Aluno criado com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateStudentRequest request)
        {
            try
            {
                var aluno = _alunoDAO.GetById(id);
                if (aluno == null)
                    return NotFound("Aluno não encontrado");

                // Remover verificação de email pois Aluno não tem mais email

                if (!string.IsNullOrEmpty(request.Cpf) && request.Cpf != aluno._cpf)
                {
                    var existingCpf = _alunoDAO.GetByCpf(request.Cpf);

                    if (existingCpf != null && existingCpf._id != id)
                    {
                        return BadRequest(new { message = "CPF já está em uso" });
                    }
                }

                aluno._telefone = request.Phone ?? aluno._telefone;
                aluno._cpf = request.Cpf ?? aluno._cpf;

                _alunoDAO.Update(aluno);

                var response = new
                {
                    id = aluno._id,
                    name = request.Name ?? "",
                    email = request.Email ?? "",
                    phone = aluno._telefone,
                    cpf = aluno._cpf,
                    paymentStatus = (string?)null,
                    route = (string?)null,
                    enrollmentDate = (string?)null,
                    status = "active",
                    createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
                };

                return Ok(new { data = response, message = "Aluno atualizado com sucesso" });
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
                var aluno = _alunoDAO.GetById(id);
                if (aluno == null)
                    return NotFound("Aluno não encontrado");

                _alunoDAO.Delete(id);
                return Ok(new { message = "Aluno removido com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }

    public class CreateStudentRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public string? Route { get; set; }
        public string? EnrollmentDate { get; set; }
    }

    public class UpdateStudentRequest
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Cpf { get; set; }
    }
}
