using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartSell.Api.Data;
using SmartSell.Api.Models.Galdino;
using System.Text.Json;

namespace SmartSell.Api.Controllers.Galdino
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController : ControllerBase
    {
        private readonly GaldinoDbContext _context;

        public AttendanceController(GaldinoDbContext context)
        {
            _context = context;
        }

        // GET: api/attendance
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAttendanceHistory(
            [FromQuery] int? studentId = null,
            [FromQuery] int? routeId = null)
        {
            var query = _context.Presencas
                .Include(p => p.Aluno)
                .Include(p => p.Rota)
                .AsQueryable();

            if (studentId.HasValue)
                query = query.Where(p => p._studentId == studentId.Value);

            if (routeId.HasValue)
                query = query.Where(p => p._routeId == routeId.Value);

            var presencas = await query.OrderByDescending(p => p._date).ToListAsync();

            var result = presencas.Select(p => new
            {
                id = p._id,
                routeId = p._routeId,
                studentId = p._studentId,
                studentName = p.Aluno?._nome ?? "N/A",
                routeName = p.Rota?._destino ?? "N/A",
                status = p._status,
                observation = p._observation,
                date = p._date.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                createdAt = p._createdAt.ToString("yyyy-MM-ddTHH:mm:ssZ")
            });

            return Ok(result);
        }

        // GET: api/attendance/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetAttendance(int id)
        {
            var presenca = await _context.Presencas
                .Include(p => p.Aluno)
                .Include(p => p.Rota)
                .FirstOrDefaultAsync(p => p._id == id);

            if (presenca == null)
            {
                return NotFound(new { error = new { message = "Registro de presença não encontrado", code = "ATTENDANCE_NOT_FOUND" } });
            }

            var result = new
            {
                id = presenca._id,
                routeId = presenca._routeId,
                studentId = presenca._studentId,
                studentName = presenca.Aluno?._nome ?? "N/A",
                routeName = presenca.Rota?._destino ?? "N/A",
                status = presenca._status,
                observation = presenca._observation,
                date = presenca._date.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                createdAt = presenca._createdAt.ToString("yyyy-MM-ddTHH:mm:ssZ")
            };

            return Ok(result);
        }

        // POST: api/attendance
        [HttpPost]
        public async Task<ActionResult<object>> MarkAttendance([FromBody] JsonElement body)
        {
            try
            {
                var routeId = body.GetProperty("routeId").GetInt32();
                var studentId = body.GetProperty("studentId").GetInt32();
                var status = body.GetProperty("status").GetString() ?? "present";
                var observation = body.TryGetProperty("observation", out var obsElement) ? obsElement.GetString() : null;

                // Verificar se o aluno existe
                var aluno = await _context.Alunos.FindAsync(studentId);
                if (aluno == null)
                {
                    return BadRequest(new { error = new { message = "Aluno não encontrado", code = "STUDENT_NOT_FOUND" } });
                }

                // Verificar se a rota existe
                var rota = await _context.Rotas.FindAsync(routeId);
                if (rota == null)
                {
                    return BadRequest(new { error = new { message = "Rota não encontrada", code = "ROUTE_NOT_FOUND" } });
                }

                // Verificar se já existe presença para hoje
                var hoje = DateTime.Today;
                var presencaExistente = await _context.Presencas
                    .FirstOrDefaultAsync(p => p._studentId == studentId && 
                                            p._routeId == routeId && 
                                            p._date.Date == hoje);

                if (presencaExistente != null)
                {
                    // Atualizar presença existente
                    presencaExistente._status = status;
                    presencaExistente._observation = observation;
                    await _context.SaveChangesAsync();

                    var resultUpdate = new
                    {
                        id = presencaExistente._id,
                        routeId = presencaExistente._routeId,
                        studentId = presencaExistente._studentId,
                        studentName = aluno._nome,
                        routeName = rota._destino,
                        status = presencaExistente._status,
                        observation = presencaExistente._observation,
                        date = presencaExistente._date.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        createdAt = presencaExistente._createdAt.ToString("yyyy-MM-ddTHH:mm:ssZ")
                    };

                    return Ok(resultUpdate);
                }

                // Criar nova presença
                var presenca = new Presenca
                {
                    _routeId = routeId,
                    _studentId = studentId,
                    _status = status,
                    _observation = observation,
                    _date = DateTime.Now,
                    _createdAt = DateTime.Now
                };

                _context.Presencas.Add(presenca);
                await _context.SaveChangesAsync();

                var result = new
                {
                    id = presenca._id,
                    routeId = presenca._routeId,
                    studentId = presenca._studentId,
                    studentName = aluno._nome,
                    routeName = rota._destino,
                    status = presenca._status,
                    observation = presenca._observation,
                    date = presenca._date.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    createdAt = presenca._createdAt.ToString("yyyy-MM-ddTHH:mm:ssZ")
                };

                return CreatedAtAction(nameof(GetAttendance), new { id = presenca._id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = new { message = "Dados inválidos", code = "INVALID_DATA", details = ex.Message } });
            }
        }

        // PUT: api/attendance/5
        [HttpPut("{id}")]
        public async Task<ActionResult<object>> UpdateAttendance(int id, [FromBody] JsonElement body)
        {
            var presenca = await _context.Presencas
                .Include(p => p.Aluno)
                .Include(p => p.Rota)
                .FirstOrDefaultAsync(p => p._id == id);

            if (presenca == null)
            {
                return NotFound(new { error = new { message = "Registro de presença não encontrado", code = "ATTENDANCE_NOT_FOUND" } });
            }

            try
            {
                if (body.TryGetProperty("status", out var statusElement))
                    presenca._status = statusElement.GetString() ?? presenca._status;

                if (body.TryGetProperty("observation", out var observationElement))
                    presenca._observation = observationElement.GetString();

                await _context.SaveChangesAsync();

                var result = new
                {
                    id = presenca._id,
                    routeId = presenca._routeId,
                    studentId = presenca._studentId,
                    studentName = presenca.Aluno?._nome ?? "N/A",
                    routeName = presenca.Rota?._destino ?? "N/A",
                    status = presenca._status,
                    observation = presenca._observation,
                    date = presenca._date.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    createdAt = presenca._createdAt.ToString("yyyy-MM-ddTHH:mm:ssZ")
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = new { message = "Dados inválidos", code = "INVALID_DATA", details = ex.Message } });
            }
        }

        // DELETE: api/attendance/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttendance(int id)
        {
            var presenca = await _context.Presencas.FindAsync(id);
            if (presenca == null)
            {
                return NotFound(new { error = new { message = "Registro de presença não encontrado", code = "ATTENDANCE_NOT_FOUND" } });
            }

            _context.Presencas.Remove(presenca);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/attendance/student/5/summary
        [HttpGet("student/{studentId}/summary")]
        public async Task<ActionResult<object>> GetStudentAttendanceSummary(int studentId)
        {
            var aluno = await _context.Alunos.FindAsync(studentId);
            if (aluno == null)
            {
                return NotFound(new { error = new { message = "Aluno não encontrado", code = "STUDENT_NOT_FOUND" } });
            }

            var presencas = await _context.Presencas
                .Where(p => p._studentId == studentId)
                .ToListAsync();

            var totalPresencas = presencas.Count;
            var presentes = presencas.Count(p => p._status == "present");
            var ausentes = presencas.Count(p => p._status == "absent");
            var percentualPresenca = totalPresencas > 0 ? (double)presentes / totalPresencas * 100 : 0;

            var result = new
            {
                studentId = studentId,
                studentName = aluno._nome,
                totalRecords = totalPresencas,
                present = presentes,
                absent = ausentes,
                attendancePercentage = Math.Round(percentualPresenca, 2)
            };

            return Ok(result);
        }
    }
}
