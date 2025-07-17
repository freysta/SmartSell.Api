using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartSell.Api.DAO;
using SmartSell.Api.Data;
using SmartSell.Api.Models.Galdino;
using System.Text.Json;

namespace SmartSell.Api.Controllers.Galdino
{
    [ApiController]
    [Route("api/notifications")]
    public class NotificationsController : ControllerBase
    {
        private readonly NotificacaoDAO _notificacaoDAO;
        private readonly AlunoDAO _alunoDAO;

        public NotificationsController(GaldinoDbContext context)
        {
            _notificacaoDAO = new NotificacaoDAO(context);
            _alunoDAO = new AlunoDAO(context);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetNotifications(
            [FromQuery] string? type = null,
            [FromQuery] int? targetId = null)
        {
            var notificacoes = _notificacaoDAO.GetAll();

            if (!string.IsNullOrEmpty(type))
                notificacoes = notificacoes.Where(n => n._tipo == type).ToList();

            if (targetId.HasValue)
                notificacoes = notificacoes.Where(n => n._alunoId == targetId.Value).ToList();

            var result = notificacoes.Select(n => new
            {
                id = n._id,
                title = n._titulo,
                message = n._mensagem,
                type = n._tipo,
                priority = "normal",
                targetType = "student",
                targetIds = n._alunoId.HasValue ? new[] { n._alunoId.Value } : new int[0],
                createdAt = n._dataEnvio.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                readBy = new int[0]
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetNotification(int id)
        {
            var notificacao = _notificacaoDAO.GetById(id);

            if (notificacao == null)
            {
                return NotFound(new { error = new { message = "Notificação não encontrada", code = "NOTIFICATION_NOT_FOUND" } });
            }

            var result = new
            {
                id = notificacao._id,
                title = notificacao._titulo,
                message = notificacao._mensagem,
                type = notificacao._tipo,
                priority = "normal",
                targetType = "student",
                targetIds = notificacao._alunoId.HasValue ? new[] { notificacao._alunoId.Value } : new int[0],
                createdAt = notificacao._dataEnvio.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                readBy = new int[0]
            };

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<object>> CreateNotification([FromBody] JsonElement body)
        {
            try
            {
                var title = body.GetProperty("title").GetString() ?? "";
                var message = body.GetProperty("message").GetString() ?? "";
                var type = body.GetProperty("type").GetString() ?? "Informativo";

                var notificacao = new Notificacao
                {
                    _titulo = title,
                    _mensagem = message,
                    _tipo = type,
                    _dataEnvio = DateTime.Now,
                    _lida = false
                };

                if (body.TryGetProperty("targetIds", out var targetIdsElement) && targetIdsElement.GetArrayLength() > 0)
                {
                    var targetId = targetIdsElement[0].GetInt32();
                    var aluno = _alunoDAO.GetById(targetId);
                    if (aluno != null)
                    {
                        notificacao._alunoId = targetId;
                    }
                }

                _notificacaoDAO.Create(notificacao);

                var result = new
                {
                    id = notificacao._id,
                    title = notificacao._titulo,
                    message = notificacao._mensagem,
                    type = notificacao._tipo,
                    priority = "normal",
                    targetType = "student",
                    targetIds = notificacao._alunoId.HasValue ? new[] { notificacao._alunoId.Value } : new int[0],
                    createdAt = notificacao._dataEnvio.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    readBy = new int[0]
                };

                return CreatedAtAction(nameof(GetNotification), new { id = notificacao._id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = new { message = "Dados inválidos", code = "INVALID_DATA", details = ex.Message } });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<object>> UpdateNotification(int id, [FromBody] JsonElement body)
        {
            var notificacao = _notificacaoDAO.GetById(id);
            if (notificacao == null)
            {
                return NotFound(new { error = new { message = "Notificação não encontrada", code = "NOTIFICATION_NOT_FOUND" } });
            }

            try
            {
                if (body.TryGetProperty("title", out var titleElement))
                    notificacao._titulo = titleElement.GetString() ?? notificacao._titulo;

                if (body.TryGetProperty("message", out var messageElement))
                    notificacao._mensagem = messageElement.GetString() ?? notificacao._mensagem;

                if (body.TryGetProperty("type", out var typeElement))
                    notificacao._tipo = typeElement.GetString() ?? notificacao._tipo;

                _notificacaoDAO.Update(notificacao);

                var result = new
                {
                    id = notificacao._id,
                    title = notificacao._titulo,
                    message = notificacao._mensagem,
                    type = notificacao._tipo,
                    priority = "normal",
                    targetType = "student",
                    targetIds = notificacao._alunoId.HasValue ? new[] { notificacao._alunoId.Value } : new int[0],
                    createdAt = notificacao._dataEnvio.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    readBy = new int[0]
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = new { message = "Dados inválidos", code = "INVALID_DATA", details = ex.Message } });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var notificacao = _notificacaoDAO.GetById(id);
            if (notificacao == null)
            {
                return NotFound(new { error = new { message = "Notificação não encontrada", code = "NOTIFICATION_NOT_FOUND" } });
            }

            _notificacaoDAO.Delete(id);

            return NoContent();
        }

        [HttpPost("{id}/mark-read")]
        public async Task<ActionResult<object>> MarkAsRead(int id, [FromBody] JsonElement body)
        {
            var notificacao = _notificacaoDAO.GetById(id);
            if (notificacao == null)
            {
                return NotFound(new { error = new { message = "Notificação não encontrada", code = "NOTIFICATION_NOT_FOUND" } });
            }

            try
            {
                notificacao._lida = true;
                _notificacaoDAO.Update(notificacao);

                return Ok(new { message = "Notificação marcada como lida" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = new { message = "Erro ao marcar como lida", code = "MARK_READ_ERROR", details = ex.Message } });
            }
        }
    }
}
