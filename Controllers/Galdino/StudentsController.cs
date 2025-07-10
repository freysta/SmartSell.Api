using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartSell.Api.Data;
using SmartSell.Api.DTOs;
using SmartSell.Api.Models.Galdino;

namespace SmartSell.Api.Controllers.Galdino
{
    [ApiController]
    [Route("api/students")]
    [Authorize]
    public class StudentsController : ControllerBase
    {
        private readonly GaldinoDbContext _context;

        public StudentsController(GaldinoDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentDto>>> GetStudents(
            [FromQuery] string? status = null,
            [FromQuery] string? route = null)
        {
            try
            {
                var query = _context.Alunos.AsQueryable();

                // Aplicar filtros se fornecidos
                if (!string.IsNullOrEmpty(status))
                {
                    // Implementar lógica de filtro por status se necessário
                }

                if (!string.IsNullOrEmpty(route))
                {
                    // Implementar lógica de filtro por rota se necessário
                }

                var alunos = await query.ToListAsync();

                var studentDtos = alunos.Select(a => new StudentDto
                {
                    Id = a.IdAluno,
                    Name = a.Nome,
                    Email = a.Email,
                    Phone = a.Telefone,
                    Cpf = a.Cpf,
                    PaymentStatus = GetPaymentStatus(a.IdAluno),
                    Route = "Campus Norte", // Implementar lógica real
                    EnrollmentDate = DateTime.Now.AddDays(-30), // Implementar campo real
                    Status = "active",
                    CreatedAt = DateTime.Now.AddDays(-30)
                }).ToList();

                return Ok(studentDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StudentDto>> GetStudent(int id)
        {
            try
            {
                var aluno = await _context.Alunos.FindAsync(id);

                if (aluno == null)
                {
                    return NotFound(new { message = "Estudante não encontrado" });
                }

                var studentDto = new StudentDto
                {
                    Id = aluno.IdAluno,
                    Name = aluno.Nome,
                    Email = aluno.Email,
                    Phone = aluno.Telefone,
                    Cpf = aluno.Cpf,
                    PaymentStatus = GetPaymentStatus(aluno.IdAluno),
                    Route = "Campus Norte",
                    EnrollmentDate = DateTime.Now.AddDays(-30),
                    Status = "active",
                    CreatedAt = DateTime.Now.AddDays(-30)
                };

                return Ok(studentDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<StudentDto>> CreateStudent([FromBody] CreateStudentDto createDto)
        {
            try
            {
                // Verificar se CPF já existe
                var existingStudent = await _context.Alunos
                    .FirstOrDefaultAsync(a => a.Cpf == createDto.Cpf);

                if (existingStudent != null)
                {
                    return BadRequest(new { message = "CPF já cadastrado" });
                }

                var aluno = new Aluno
                {
                    Nome = createDto.Name,
                    Email = createDto.Email,
                    Telefone = createDto.Phone,
                    Cpf = createDto.Cpf
                };

                _context.Alunos.Add(aluno);
                await _context.SaveChangesAsync();

                var studentDto = new StudentDto
                {
                    Id = aluno.IdAluno,
                    Name = aluno.Nome,
                    Email = aluno.Email,
                    Phone = aluno.Telefone,
                    Cpf = aluno.Cpf,
                    PaymentStatus = "pending",
                    Route = createDto.Route ?? "Campus Norte",
                    EnrollmentDate = createDto.EnrollmentDate ?? DateTime.Now,
                    Status = "active",
                    CreatedAt = DateTime.Now
                };

                return CreatedAtAction(nameof(GetStudent), new { id = aluno.IdAluno }, studentDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] UpdateStudentDto updateDto)
        {
            try
            {
                var aluno = await _context.Alunos.FindAsync(id);

                if (aluno == null)
                {
                    return NotFound(new { message = "Estudante não encontrado" });
                }

                // Verificar se CPF já existe em outro estudante
                if (!string.IsNullOrEmpty(updateDto.Cpf) && updateDto.Cpf != aluno.Cpf)
                {
                    var existingStudent = await _context.Alunos
                        .FirstOrDefaultAsync(a => a.Cpf == updateDto.Cpf && a.IdAluno != id);

                    if (existingStudent != null)
                    {
                        return BadRequest(new { message = "CPF já cadastrado para outro estudante" });
                    }
                }

                // Atualizar campos
                if (!string.IsNullOrEmpty(updateDto.Name))
                    aluno.Nome = updateDto.Name;

                if (updateDto.Email != null)
                    aluno.Email = updateDto.Email;

                if (updateDto.Phone != null)
                    aluno.Telefone = updateDto.Phone;

                if (!string.IsNullOrEmpty(updateDto.Cpf))
                    aluno.Cpf = updateDto.Cpf;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            try
            {
                var aluno = await _context.Alunos.FindAsync(id);

                if (aluno == null)
                {
                    return NotFound(new { message = "Estudante não encontrado" });
                }

                _context.Alunos.Remove(aluno);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        private string GetPaymentStatus(int alunoId)
        {
            // Implementar lógica real para verificar status de pagamento
            var random = new Random();
            var statuses = new[] { "paid", "pending", "overdue" };
            return statuses[random.Next(statuses.Length)];
        }
    }
}
