using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartSell.Api.DAO;
using SmartSell.Api.Data;
using SmartSell.Api.Models.Galdino;
using System.Text.Json;

namespace SmartSell.Api.Controllers.Galdino
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController : ControllerBase
    {
        private readonly PresencaDAO _presencaDAO;
        private readonly AlunoDAO _alunoDAO;
        private readonly RotaDAO _rotaDAO;

        public AttendanceController(GaldinoDbContext context)
        {
            _presencaDAO = new PresencaDAO(context);
            _alunoDAO = new AlunoDAO(context);
            _rotaDAO = new RotaDAO(context);
        }

        // GET: api/attendance
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAttendanceHistory(
            [FromQuery] int? studentId = null,
            [FromQuery] int? routeId = null)
        {
            var presencas = _presencaDAO.GetAll();

            if (studentId.HasValue)
                presencas = presencas.Where(p => p._alunoId == studentId.Value).ToList();

            if (routeId.HasValue)
                presencas = presencas.Where(p => p._rotaId == routeId.Value).ToList();

            var result = presencas.Select(p => new
            {
                id = p._id,
                routeId = p._rotaId,
                studentId = p._alunoId,
                studentName = "N/A",
                routeName = "N/A",
                status = p._presente,
                observation = p._observacao,
                date = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
            });

            return Ok(result);
        }

        // GET: api/attendance/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetAttendance(int id)
        {
            var presenca = _presencaDAO.GetById(id);

            if (presenca == null)
            {
                return NotFound(new { error = new { message = "Registro de presença não encontrado", code = "ATTENDANCE_NOT_FOUND" } });
            }

            var result = new
            {
                id = presenca._id,
                routeId = presenca._rotaId,
                studentId = presenca._alunoId,
                studentName = "N/A",
                routeName = "N/A",
                status = presenca._presente,
                observation = presenca._observacao,
                date = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
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
                var aluno = _alunoDAO.GetById(studentId);
                if (aluno == null)
                {
                    return BadRequest(new { error = new { message = "Aluno não encontrado", code = "STUDENT_NOT_FOUND" } });
                }

                // Verificar se a rota existe
                var rota = _rotaDAO.GetById(routeId);
                if (rota == null)
                {
                    return BadRequest(new { error = new { message = "Rota não encontrada", code = "ROUTE_NOT_FOUND" } });
                }

                // Criar nova presença
                var presenca = new Presenca
                {
                    _rotaId = routeId,
                    _alunoId = studentId,
                    _presente = status,
                    _observacao = observation
                };

                _presencaDAO.Create(presenca);

                var result = new
                {
                    id = presenca._id,
                    routeId = presenca._rotaId,
                    studentId = presenca._alunoId,
                    studentName = "N/A",
                    routeName = "N/A",
                    status = presenca._presente,
                    observation = presenca._observacao,
                    date = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
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
            var presenca = _presencaDAO.GetById(id);

            if (presenca == null)
            {
                return NotFound(new { error = new { message = "Registro de presença não encontrado", code = "ATTENDANCE_NOT_FOUND" } });
            }

            try
            {
                if (body.TryGetProperty("status", out var statusElement))
                    presenca._presente = statusElement.GetString() ?? presenca._presente;

                if (body.TryGetProperty("observation", out var observationElement))
                    presenca._observacao = observationElement.GetString();

                _presencaDAO.Update(presenca);

                var result = new
                {
                    id = presenca._id,
                    routeId = presenca._rotaId,
                    studentId = presenca._alunoId,
                    studentName = "N/A",
                    routeName = "N/A",
                    status = presenca._presente,
                    observation = presenca._observacao,
                    date = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
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
            var presenca = _presencaDAO.GetById(id);
            if (presenca == null)
            {
                return NotFound(new { error = new { message = "Registro de presença não encontrado", code = "ATTENDANCE_NOT_FOUND" } });
            }

            _presencaDAO.Delete(id);

            return NoContent();
        }

        // GET: api/attendance/student/5/summary
        [HttpGet("student/{studentId}/summary")]
        public async Task<ActionResult<object>> GetStudentAttendanceSummary(int studentId)
        {
            var aluno = _alunoDAO.GetById(studentId);
            if (aluno == null)
            {
                return NotFound(new { error = new { message = "Aluno não encontrado", code = "STUDENT_NOT_FOUND" } });
            }

            var presencas = _presencaDAO.GetAll()
                .Where(p => p._alunoId == studentId)
                .ToList();

            var totalPresencas = presencas.Count;
            var presentes = presencas.Count(p => p._presente == "Sim");
            var ausentes = presencas.Count(p => p._presente == "Não");
            var percentualPresenca = totalPresencas > 0 ? (double)presentes / totalPresencas * 100 : 0;

            var result = new
            {
                studentId = studentId,
                studentName = "N/A",
                totalRecords = totalPresencas,
                present = presentes,
                absent = ausentes,
                attendancePercentage = Math.Round(percentualPresenca, 2)
            };

            return Ok(result);
        }
    }
}
