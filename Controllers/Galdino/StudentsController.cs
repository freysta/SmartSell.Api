using Microsoft.AspNetCore.Mvc;
using SmartSell.Api.Data;
using SmartSell.Api.Models.Galdino;

namespace SmartSell.Api.Controllers.Galdino
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly GaldinoDbContext _context;

        public StudentsController(GaldinoDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] string? status, [FromQuery] string? route)
        {
            try
            {
                var query = _context.Alunos.AsQueryable();

                if (!string.IsNullOrEmpty(status))
                {
                }

                if (!string.IsNullOrEmpty(route))
                {
                }

                var alunos = query.Select(a => new
                {
                    id = a._id,
                    name = a._nome,
                    email = a._email,
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
                var aluno = _context.Alunos.Find(id);
                if (aluno == null)
                    return NotFound("Aluno não encontrado");

                var response = new
                {
                    id = aluno._id,
                    name = aluno._nome,
                    email = aluno._email,
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
                var existingStudent = _context.Alunos
                    .FirstOrDefault(a => a._email == request.Email);

                if (existingStudent != null)
                {
                    return BadRequest(new { message = "Email já está em uso" });
                }

                var existingCpf = _context.Alunos
                    .FirstOrDefault(a => a._cpf == request.Cpf);

                if (existingCpf != null)
                {
                    return BadRequest(new { message = "CPF já está em uso" });
                }

                var aluno = new Aluno
                {
                    _nome = request.Name,
                    _email = request.Email,
                    _telefone = request.Phone,
                    _cpf = request.Cpf
                };

                _context.Alunos.Add(aluno);
                _context.SaveChanges();

                var response = new
                {
                    id = aluno._id,
                    name = aluno._nome,
                    email = aluno._email,
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
                var aluno = _context.Alunos.Find(id);
                if (aluno == null)
                    return NotFound("Aluno não encontrado");

                if (!string.IsNullOrEmpty(request.Email) && request.Email != aluno._email)
                {
                    var existingEmail = _context.Alunos
                        .FirstOrDefault(a => a._email == request.Email && a._id != id);

                    if (existingEmail != null)
                    {
                        return BadRequest(new { message = "Email já está em uso" });
                    }
                }

                if (!string.IsNullOrEmpty(request.Cpf) && request.Cpf != aluno._cpf)
                {
                    var existingCpf = _context.Alunos
                        .FirstOrDefault(a => a._cpf == request.Cpf && a._id != id);

                    if (existingCpf != null)
                    {
                        return BadRequest(new { message = "CPF já está em uso" });
                    }
                }

                aluno._nome = request.Name ?? aluno._nome;
                aluno._email = request.Email ?? aluno._email;
                aluno._telefone = request.Phone ?? aluno._telefone;
                aluno._cpf = request.Cpf ?? aluno._cpf;

                _context.SaveChanges();

                var response = new
                {
                    id = aluno._id,
                    name = aluno._nome,
                    email = aluno._email,
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
                var aluno = _context.Alunos.Find(id);
                if (aluno == null)
                    return NotFound("Aluno não encontrado");

                _context.Alunos.Remove(aluno);
                _context.SaveChanges();
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
