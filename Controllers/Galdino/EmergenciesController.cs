using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartSell.Api.Data;
using SmartSell.Api.Models.Galdino;
using System.Text.Json;

namespace SmartSell.Api.Controllers.Galdino
{
    [ApiController]
    [Route("api/emergencies")]
    public class EmergenciesController : ControllerBase
    {
        private readonly GaldinoDbContext _context;

        public EmergenciesController(GaldinoDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmergencies()
        {
            try
            {
                var emergencies = _context.Emergencias.ToList();
                return Ok(emergencies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = new { message = "Erro interno do servidor", details = ex.Message } });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmergency(int id)
        {
            try
            {
                var emergency = _context.Emergencias.Find(id);
                if (emergency == null)
                {
                    return NotFound(new { error = new { message = "Emergência não encontrada" } });
                }
                return Ok(emergency);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = new { message = "Erro interno do servidor", details = ex.Message } });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmergency([FromBody] JsonElement body)
        {
            try
            {
                var emergency = new Emergencia
                {
                    _rotaId = body.TryGetProperty("routeId", out var routeId) ? routeId.GetInt32() : 1,
                    _tipoEmergencia = body.GetProperty("type").GetString() ?? "",
                    _descricao = body.GetProperty("description").GetString() ?? "",
                    _dataHora = DateTime.Now,
                    _resolvido = false,
                    _observacoes = body.TryGetProperty("location", out var location) ? location.GetString() : null
                };

                _context.Emergencias.Add(emergency);
                _context.SaveChanges();

                return CreatedAtAction(nameof(GetEmergency), new { id = emergency._id }, emergency);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = new { message = "Dados inválidos", code = "INVALID_DATA", details = ex.Message } });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmergency(int id, [FromBody] JsonElement body)
        {
            try
            {
                var emergency = _context.Emergencias.Find(id);
                if (emergency == null)
                {
                    return NotFound(new { error = new { message = "Emergência não encontrada" } });
                }

                if (body.TryGetProperty("type", out var type))
                    emergency._tipoEmergencia = type.GetString() ?? emergency._tipoEmergencia;

                if (body.TryGetProperty("description", out var description))
                    emergency._descricao = description.GetString() ?? emergency._descricao;

                if (body.TryGetProperty("location", out var location))
                    emergency._observacoes = location.GetString() ?? emergency._observacoes;

                if (body.TryGetProperty("status", out var status))
                    emergency._resolvido = status.GetString() == "Resolvida";

                _context.SaveChanges();

                return Ok(emergency);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = new { message = "Dados inválidos", code = "INVALID_DATA", details = ex.Message } });
            }
        }

        [HttpPost("{id}/resolve")]
        public async Task<IActionResult> ResolveEmergency(int id)
        {
            try
            {
                var emergency = _context.Emergencias.Find(id);
                if (emergency == null)
                {
                    return NotFound(new { error = new { message = "Emergência não encontrada" } });
                }

                emergency._resolvido = true;
                emergency._observacoes = "Emergência resolvida em " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                _context.SaveChanges();

                return Ok(new { message = "Emergência resolvida com sucesso", emergency });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = new { message = "Erro interno do servidor", details = ex.Message } });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmergency(int id)
        {
            try
            {
                var emergency = _context.Emergencias.Find(id);
                if (emergency == null)
                {
                    return NotFound(new { error = new { message = "Emergência não encontrada" } });
                }

                _context.Emergencias.Remove(emergency);
                _context.SaveChanges();

                return Ok(new { message = "Emergência excluída com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = new { message = "Erro interno do servidor", details = ex.Message } });
            }
        }
    }
}
