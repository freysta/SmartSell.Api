using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartSell.Api.Data;
using SmartSell.Api.DTOs;
using SmartSell.Api.Models.Galdino;

namespace SmartSell.Api.Controllers.Galdino
{
    [ApiController]
    [Route("api/[controller]")]
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
                .Include(ra => ra.Aluno)
                .Include(ra => ra.Rota)
                .Include(ra => ra.PontoEmbarque)
                .Select(ra => new RotaAlunoResponseDto
                {
                    Id = ra._id,
                    FkIdRota = ra._fkIdRota,
                    FkIdAluno = ra._fkIdAluno,
                    FkIdPonto = ra._fkIdPonto,
                    Confirmado = ra._confirmado,
                    NomeAluno = ra.Aluno != null ? ra.Aluno._nome : null,
                    DestinoRota = ra.Rota != null ? ra.Rota._destino : null,
                    NomePonto = ra.PontoEmbarque != null ? ra.PontoEmbarque._name : null
                })
                .ToListAsync();

            return Ok(rotaAlunos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RotaAlunoResponseDto>> GetRotaAluno(int id)
        {
            var rotaAluno = await _context.RotaAlunos
                .Include(ra => ra.Aluno)
                .Include(ra => ra.Rota)
                .Include(ra => ra.PontoEmbarque)
                .Where(ra => ra._id == id)
                .Select(ra => new RotaAlunoResponseDto
                {
                    Id = ra._id,
                    FkIdRota = ra._fkIdRota,
                    FkIdAluno = ra._fkIdAluno,
                    FkIdPonto = ra._fkIdPonto,
                    Confirmado = ra._confirmado,
                    NomeAluno = ra.Aluno != null ? ra.Aluno._nome : null,
                    DestinoRota = ra.Rota != null ? ra.Rota._destino : null,
                    NomePonto = ra.PontoEmbarque != null ? ra.PontoEmbarque._name : null
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
                .Include(ra => ra.Aluno)
                .Include(ra => ra.PontoEmbarque)
                .Where(ra => ra._fkIdRota == rotaId)
                .Select(ra => new RotaAlunoResponseDto
                {
                    Id = ra._id,
                    FkIdRota = ra._fkIdRota,
                    FkIdAluno = ra._fkIdAluno,
                    FkIdPonto = ra._fkIdPonto,
                    Confirmado = ra._confirmado,
                    NomeAluno = ra.Aluno != null ? ra.Aluno._nome : null,
                    NomePonto = ra.PontoEmbarque != null ? ra.PontoEmbarque._name : null
                })
                .ToListAsync();

            return Ok(rotaAlunos);
        }

        [HttpGet("aluno/{alunoId}")]
        public async Task<ActionResult<IEnumerable<RotaAlunoResponseDto>>> GetRotasByAluno(int alunoId)
        {
            var rotaAlunos = await _context.RotaAlunos
                .Include(ra => ra.Rota)
                .Include(ra => ra.PontoEmbarque)
                .Where(ra => ra._fkIdAluno == alunoId)
                .Select(ra => new RotaAlunoResponseDto
                {
                    Id = ra._id,
                    FkIdRota = ra._fkIdRota,
                    FkIdAluno = ra._fkIdAluno,
                    FkIdPonto = ra._fkIdPonto,
                    Confirmado = ra._confirmado,
                    DestinoRota = ra.Rota != null ? ra.Rota._destino : null,
                    NomePonto = ra.PontoEmbarque != null ? ra.PontoEmbarque._name : null
                })
                .ToListAsync();

            return Ok(rotaAlunos);
        }

        [HttpPost]
        public async Task<ActionResult<RotaAlunoResponseDto>> CreateRotaAluno(CreateRotaAlunoDto dto)
        {
            var rotaAluno = new RotaAluno
            {
                _fkIdRota = dto.FkIdRota,
                _fkIdAluno = dto.FkIdAluno,
                _fkIdPonto = dto.FkIdPonto,
                _confirmado = dto.Confirmado
            };

            _context.RotaAlunos.Add(rotaAluno);
            await _context.SaveChangesAsync();

            var response = await _context.RotaAlunos
                .Include(ra => ra.Aluno)
                .Include(ra => ra.Rota)
                .Include(ra => ra.PontoEmbarque)
                .Where(ra => ra._id == rotaAluno._id)
                .Select(ra => new RotaAlunoResponseDto
                {
                    Id = ra._id,
                    FkIdRota = ra._fkIdRota,
                    FkIdAluno = ra._fkIdAluno,
                    FkIdPonto = ra._fkIdPonto,
                    Confirmado = ra._confirmado,
                    NomeAluno = ra.Aluno != null ? ra.Aluno._nome : null,
                    DestinoRota = ra.Rota != null ? ra.Rota._destino : null,
                    NomePonto = ra.PontoEmbarque != null ? ra.PontoEmbarque._name : null
                })
                .FirstOrDefaultAsync();

            return CreatedAtAction(nameof(GetRotaAluno), new { id = rotaAluno._id }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRotaAluno(int id, UpdateRotaAlunoDto dto)
        {
            var rotaAluno = await _context.RotaAlunos.FindAsync(id);
            if (rotaAluno == null)
            {
                return NotFound();
            }

            rotaAluno._fkIdRota = dto.FkIdRota;
            rotaAluno._fkIdAluno = dto.FkIdAluno;
            rotaAluno._fkIdPonto = dto.FkIdPonto;
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
