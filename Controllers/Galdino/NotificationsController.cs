using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartSell.Api.Data;
using SmartSell.Api.Models.Galdino;
using System.Text.Json;

namespace SmartSell.Api.Controllers.Galdino
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly GaldinoDbContext _context;

        public NotificationsController(GaldinoDbContext context)
        {
            _context = context;
        }

        // GET: api/notifications
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetNotifications([FromQuery] int? userId = null)
        {
            var query = _context.Notificacoes.AsQueryable();

            // Se userId for especificado, filtrar notificações relevantes
            if (userId.HasValue)
            {
                query = query.Where(n => 
                    n._targetType == "all" || 
                    (n._targetType == "specific" && n._targetIds != null && n._targetIds.Contains(userId.Value.ToString())));
            }

            var notificacoes = await query.OrderByDescending(n => n._createdAt).ToListAsync();

            var result = notificacoes.Select(n => new
            {
                id = n._id,
                title = n._title,
                message = n._message,
                type = n._type,
                priority = n._priority,
                targetType = n._targetType,
                targetIds = ParseJsonArray(n._targetIds),
                createdAt = n._createdAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                readBy = ParseJsonArray(n._readBy)
            });

            return Ok(result);
        }

        // GET: api/notifications/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetNotification(int id)
        {
            var notificacao = await _context.Notificacoes.FindAsync(id);

            if (notificacao == null)
            {
                return NotFound(new { error = new { message = "Notificação não encontrada", code = "NOTIFICATION_NOT_FOUND" } });
            }

            var result = new
            {
                id = notificacao._id,
                title = notificacao._title,
                message = notificacao._message,
                type = notificacao._type,
                priority = notificacao._priority,
                targetType = notificacao._targetType,
                targetIds = ParseJsonArray(notificacao._targetIds),
                createdAt = notificacao._createdAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                readBy = ParseJsonArray(notificacao._readBy)
            };

            return Ok(result);
        }

        // POST: api/notifications
        [HttpPost]
        public async Task<ActionResult<object>> CreateNotification([FromBody] JsonElement body)
        {
            try
            {
                var notificacao = new Notificacao
                {
                    _title = body.GetProperty("title").GetString() ?? "",
                    _message = body.GetProperty("message").GetString() ?? "",
                    _type = body.GetProperty("type").GetString() ?? "info",
                    _priority = body.GetProperty("priority").GetString() ?? "normal",
                    _targetType = body.GetProperty("targetType").GetString() ?? "all",
                    _createdAt = DateTime.Now,
                    _readBy = "[]"
                };

                if (body.TryGetProperty("targetIds", out var targetIdsElement))
                {
                    var targetIds = targetIdsElement.EnumerateArray().Select(x => x.GetInt32()).ToArray();
                    notificacao._targetIds = JsonSerializer.Serialize(targetIds);
                }
                else
                {
                    notificacao._targetIds = "[]";
                }

                _context.Notificacoes.Add(notificacao);
                await _context.SaveChangesAsync();

                var result = new
                {
                    id = notificacao._id,
                    title = notificacao._title,
                    message = notificacao._message,
                    type = notificacao._type,
                    priority = notificacao._priority,
                    targetType = notificacao._targetType,
                    targetIds = ParseJsonArray(notificacao._targetIds),
                    createdAt = notificacao._createdAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    readBy = ParseJsonArray(notificacao._readBy)
                };

                return CreatedAtAction(nameof(GetNotification), new { id = notificacao._id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = new { message = "Dados inválidos", code = "INVALID_DATA", details = ex.Message } });
            }
        }

        // PATCH: api/notifications/5/read
        [HttpPatch("{id}/read")]
        public async Task<ActionResult> MarkNotificationAsRead(int id, [FromBody] JsonElement body)
        {
            var notificacao = await _context.Notificacoes.FindAsync(id);
            if (notificacao == null)
            {
                return NotFound(new { error = new { message = "Notificação não encontrada", code = "NOTIFICATION_NOT_FOUND" } });
            }

            try
            {
                var userId = body.GetProperty("userId").GetInt32();
                
                var readByList = ParseJsonArray(notificacao._readBy) ?? new int[0];
                var readBySet = new HashSet<int>(readByList);
                readBySet.Add(userId);
                
                notificacao._readBy = JsonSerializer.Serialize(readBySet.ToArray());
                
                await _context.SaveChangesAsync();

                return Ok(new { message = "Notificação marcada como lida" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = new { message = "Dados inválidos", code = "INVALID_DATA", details = ex.Message } });
            }
        }

        // PUT: api/notifications/5
        [HttpPut("{id}")]
        public async Task<ActionResult<object>> UpdateNotification(int id, [FromBody] JsonElement body)
        {
            var notificacao = await _context.Notificacoes.FindAsync(id);
            if (notificacao == null)
            {
                return NotFound(new { error = new { message = "Notificação não encontrada", code = "NOTIFICATION_NOT_FOUND" } });
            }

            try
            {
                if (body.TryGetProperty("title", out var titleElement))
                    notificacao._title = titleElement.GetString() ?? notificacao._title;

                if (body.TryGetProperty("message", out var messageElement))
                    notificacao._message = messageElement.GetString() ?? notificacao._message;

                if (body.TryGetProperty("type", out var typeElement))
                    notificacao._type = typeElement.GetString() ?? notificacao._type;

                if (body.TryGetProperty("priority", out var priorityElement))
                    notificacao._priority = priorityElement.GetString() ?? notificacao._priority;

                await _context.SaveChangesAsync();

                var result = new
                {
                    id = notificacao._id,
                    title = notificacao._title,
                    message = notificacao._message,
                    type = notificacao._type,
                    priority = notificacao._priority,
                    targetType = notificacao._targetType,
                    targetIds = ParseJsonArray(notificacao._targetIds),
                    createdAt = notificacao._createdAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    readBy = ParseJsonArray(notificacao._readBy)
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = new { message = "Dados inválidos", code = "INVALID_DATA", details = ex.Message } });
            }
        }

        // DELETE: api/notifications/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var notificacao = await _context.Notificacoes.FindAsync(id);
            if (notificacao == null)
            {
                return NotFound(new { error = new { message = "Notificação não encontrada", code = "NOTIFICATION_NOT_FOUND" } });
            }

            _context.Notificacoes.Remove(notificacao);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private int[]? ParseJsonArray(string? jsonString)
        {
            if (string.IsNullOrEmpty(jsonString))
                return new int[0];

            try
            {
                return JsonSerializer.Deserialize<int[]>(jsonString);
            }
            catch
            {
                return new int[0];
            }
        }
    }
}
