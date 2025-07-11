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
        public async Task<ActionResult<List<StudentDto>>> GetStudents()
        {
            var alunos = await _context.Alunos.ToListAsync();

            var alunosDto = alunos.Select(a => new StudentDto
            {
                Id = a.IdAluno,
                Name = a.Nome,
                Email = a.Email ?? "",
                Phone = a.Telefone ?? "",
                Cpf = a.Cpf,
                PaymentStatus = "paid",
                Route = "Campus Norte",
                EnrollmentDate = DateTime.Now,
                Status = "active",
                CreatedAt = DateTime.Now
            }).ToList();

            return Ok(alunosDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StudentDto>> GetStudent(int id)
        {
            var aluno = await _context.Alunos.FindAsync(id);

            if (aluno == null)
            {
                return NotFound(new { message = "Aluno não encontrado" });
            }

            var alunoDto = new StudentDto
            {
                Id = aluno.IdAluno,
                Name = aluno.Nome,
                Email = aluno.Email ?? "",
                Phone = aluno.Telefone ?? "",
                Cpf = aluno.Cpf,
                PaymentStatus = "paid",
                Route = "Campus Norte",
                EnrollmentDate = DateTime.Now,
                Status = "active",
                CreatedAt = DateTime.Now
            };

            return Ok(alunoDto);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<StudentDto>> CreateStudent([FromBody] CreateStudentDto createDto)
        {
            var emailExists = await _context.Alunos.AnyAsync(a => a.Email == createDto.Email);
            if (emailExists)
            {
                return BadRequest(new { message = "Email já está em uso" });
            }

            var cpfExists = await _context.Alunos.AnyAsync(a => a.Cpf == createDto.Cpf);
            if (cpfExists)
            {
                return BadRequest(new { message = "CPF já está em uso" });
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

            var alunoDto = new StudentDto
            {
                Id = aluno.IdAluno,
                Name = aluno.Nome,
                Email = aluno.Email ?? "",
                Phone = aluno.Telefone ?? "",
                Cpf = aluno.Cpf,
                PaymentStatus = "paid",
                Route = createDto.Route ?? "Campus Norte",
                EnrollmentDate = createDto.EnrollmentDate ?? DateTime.Now,
                Status = "active",
                CreatedAt = DateTime.Now
            };

            return CreatedAtAction(nameof(GetStudent), new { id = aluno.IdAluno }, alunoDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<StudentDto>> UpdateStudent(int id, [FromBody] UpdateStudentDto updateDto)
        {
            var aluno = await _context.Alunos.FindAsync(id);
            if (aluno == null)
            {
                return NotFound(new { message = "Aluno não encontrado" });
            }

            if (!string.IsNullOrEmpty(updateDto.Name))
                aluno.Nome = updateDto.Name;

            if (!string.IsNullOrEmpty(updateDto.Email))
                aluno.Email = updateDto.Email;

            if (!string.IsNullOrEmpty(updateDto.Phone))
                aluno.Telefone = updateDto.Phone;

            await _context.SaveChangesAsync();

            var alunoDto = new StudentDto
            {
                Id = aluno.IdAluno,
                Name = aluno.Nome,
                Email = aluno.Email ?? "",
                Phone = aluno.Telefone ?? "",
                Cpf = aluno.Cpf,
                PaymentStatus = "paid",
                Route = updateDto.Route ?? "Campus Norte",
                EnrollmentDate = updateDto.EnrollmentDate ?? DateTime.Now,
                Status = "active",
                CreatedAt = DateTime.Now
            };

            return Ok(alunoDto);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var aluno = await _context.Alunos.FindAsync(id);
            if (aluno == null)
            {
                return NotFound(new { message = "Aluno não encontrado" });
            }

            _context.Alunos.Remove(aluno);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Aluno excluído com sucesso" });
        }
    }
}
