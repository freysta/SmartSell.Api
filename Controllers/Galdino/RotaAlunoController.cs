using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartSell.Api.Data;
using SmartSell.Api.DTOs;
using SmartSell.Api.Models.Galdino;

namespace SmartSell.Api.Controllers.Galdino
{
    [ApiController]
    [Route("api/rotaaluno")]
    public class RotaAlunoController : ControllerBase
    {
        private readonly GaldinoDbContext _context;

        public RotaAlunoController(GaldinoDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RotaAlunoResponseDto>>> GetRotaAlunos()
        {
            var rotaAlunos = await _context.RotaAlunos
                .Select(ra => new RotaAlunoResponseDto
                {
                    Id = ra._id,
                    FkIdRota = ra._rotaId,
                    FkIdAluno = ra._alunoId,
                    FkIdPonto = ra._pontoId,
                    Confirmado = ra._confirmado,
                    NomeAluno = "",
                    DestinoRota = "",
                    NomePonto = ""
                })
                .ToListAsync();

            return Ok(rotaAlunos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RotaAlunoResponseDto>> GetRotaAluno(int id)
        {
            var rotaAluno = await _context.RotaAlunos
                .Where(ra => ra._id == id)
                .Select(ra => new RotaAlunoResponseDto
                {
                    Id = ra._id,
                    FkIdRota = ra._rotaId,
                    FkIdAluno = ra._alunoId,
                    FkIdPonto = ra._pontoId,
                    Confirmado = ra._confirmado,
                    NomeAluno = "",
                    DestinoRota = "",
                    NomePonto = ""
                })
                .FirstOrDefaultAsync();

            if (rotaAluno == null)
            {
                return NotFound();
            }

            return Ok(rotaAluno);
        }

        [HttpGet("rota/{rotaId}")]
        public async Task<ActionResult<IEnumerable<RotaAlunoResponseDto>>> GetAlunosByRota(int rotaId)
        {
            var rotaAlunos = await _context.RotaAlunos
                .Where(ra => ra._rotaId == rotaId)
                .Select(ra => new RotaAlunoResponseDto
                {
                    Id = ra._id,
                    FkIdRota = ra._rotaId,
                    FkIdAluno = ra._alunoId,
                    FkIdPonto = ra._pontoId,
                    Confirmado = ra._confirmado,
                    NomeAluno = "",
                    NomePonto = ""
                })
                .ToListAsync();

            return Ok(rotaAlunos);
        }

        [HttpGet("aluno/{alunoId}")]
        public async Task<ActionResult<IEnumerable<RotaAlunoResponseDto>>> GetRotasByAluno(int alunoId)
        {
            var rotaAlunos = await _context.RotaAlunos
                .Where(ra => ra._alunoId == alunoId)
                .Select(ra => new RotaAlunoResponseDto
                {
                    Id = ra._id,
                    FkIdRota = ra._rotaId,
                    FkIdAluno = ra._alunoId,
                    FkIdPonto = ra._pontoId,
                    Confirmado = ra._confirmado,
                    DestinoRota = "",
                    NomePonto = ""
                })
                .ToListAsync();

            return Ok(rotaAlunos);
        }

        [HttpPost]
        public async Task<ActionResult<RotaAlunoResponseDto>> CreateRotaAluno(CreateRotaAlunoDto dto)
        {
            try
            {
                var sql = @"INSERT INTO rotaalunos (fk_id_rota, fk_id_aluno, fk_id_ponto, confirmado, data_confirmacao) 
                           VALUES ({0}, {1}, {2}, {3}, {4})";
                
                await _context.Database.ExecuteSqlRawAsync(sql, 
                    dto.FkIdRota, 
                    dto.FkIdAluno, 
                    dto.FkIdPonto, 
                    dto.Confirmado,
                    DateTime.Now);

                var response = new RotaAlunoResponseDto
                {
                    Id = 0,
                    FkIdRota = dto.FkIdRota,
                    FkIdAluno = dto.FkIdAluno,
                    FkIdPonto = dto.FkIdPonto,
                    Confirmado = dto.Confirmado,
                    NomeAluno = "",
                    DestinoRota = "",
                    NomePonto = ""
                };

                return Ok(new { message = "RotaAluno criado com sucesso", data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = new { message = "Erro ao criar RotaAluno", details = ex.Message } });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRotaAluno(int id, UpdateRotaAlunoDto dto)
        {
            var rotaAluno = await _context.RotaAlunos.FindAsync(id);
            if (rotaAluno == null)
            {
                return NotFound();
            }

            rotaAluno._rotaId = dto.FkIdRota;
            rotaAluno._alunoId = dto.FkIdAluno;
            rotaAluno._pontoId = dto.FkIdPonto;
            rotaAluno._confirmado = dto.Confirmado;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RotaAlunoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRotaAluno(int id)
        {
            var rotaAluno = await _context.RotaAlunos.FindAsync(id);
            if (rotaAluno == null)
            {
                return NotFound();
            }

            _context.RotaAlunos.Remove(rotaAluno);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id}/confirmar")]
        public async Task<IActionResult> ConfirmarParticipacao(int id)
        {
            var rotaAluno = await _context.RotaAlunos.FindAsync(id);
            if (rotaAluno == null)
            {
                return NotFound();
            }

            rotaAluno._confirmado = "Sim";
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id}/cancelar")]
        public async Task<IActionResult> CancelarParticipacao(int id)
        {
            var rotaAluno = await _context.RotaAlunos.FindAsync(id);
            if (rotaAluno == null)
            {
                return NotFound();
            }

            rotaAluno._confirmado = "Não";
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RotaAlunoExists(int id)
        {
            return _context.RotaAlunos.Any(e => e._id == id);
        }
    }
}
