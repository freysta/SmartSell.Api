using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartSell.Api.Data;
using SmartSell.Api.DTOs;
using SmartSell.Api.Models.Galdino;
using SmartSell.Api.Middleware;
using System.Text.Json;

namespace SmartSell.Api.Controllers.Galdino
{
    [ApiController]
    [Route("api/notificacoes")]
    [Authorize]
    public class NotificacoesController : ControllerBase
    {
        private readonly GaldinoDbContext _context;
        private readonly ILogger<NotificacoesController> _logger;

        public NotificacoesController(GaldinoDbContext context, ILogger<NotificacoesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<NotificacaoDto>>>> ObterNotificacoes(
            [FromQuery] int? usuarioId = null,
            [FromQuery] string? tipo = null,
            [FromQuery] string? prioridade = null,
            [FromQuery] bool? apenasNaoLidas = null,
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanhoPagina = 10)
        {
            try
            {
                var query = _context.Notificacoes.AsQueryable();

                // Aplicar filtros
                if (!string.IsNullOrEmpty(tipo))
                {
                    if (Enum.TryParse<TipoNotificacao>(tipo, true, out var tipoEnum))
                    {
                        query = query.Where(n => n.Tipo == tipoEnum);
                    }
                }

                if (!string.IsNullOrEmpty(prioridade))
                {
                    if (Enum.TryParse<PrioridadeNotificacao>(prioridade, true, out var prioridadeEnum))
                    {
                        query = query.Where(n => n.Prioridade == prioridadeEnum);
                    }
                }

                // Filtrar por usuário específico
                if (usuarioId.HasValue)
                {
                    query = query.Where(n => 
                        n.TipoDestino == TipoDestinoNotificacao.Todos ||
                        (n.TipoDestino == TipoDestinoNotificacao.Especifico && 
                         !string.IsNullOrEmpty(n.IdsDestino) && 
                         n.IdsDestino.Contains(usuarioId.Value.ToString())));
                }

                var totalItens = await query.CountAsync();
                var notificacoes = await query
                    .OrderByDescending(n => n.DataCriacao)
                    .Skip((pagina - 1) * tamanhoPagina)
                    .Take(tamanhoPagina)
                    .ToListAsync();

                var notificacoesDto = notificacoes.Select(n => new NotificacaoDto
                {
                    Id = n.IdNotificacao,
                    Titulo = n.Titulo,
                    Mensagem = n.Mensagem,
                    Tipo = n.Tipo.ToString().ToLower(),
                    Prioridade = n.Prioridade.ToString().ToLower(),
                    TipoDestino = n.TipoDestino.ToString().ToLower(),
                    IdsDestino = string.IsNullOrEmpty(n.IdsDestino) ? 
                        null : 
                        JsonSerializer.Deserialize<List<int>>(n.IdsDestino),
                    DataCriacao = n.DataCriacao,
                    LidaPor = string.IsNullOrEmpty(n.LidaPor) ? 
                        new List<int>() : 
                        JsonSerializer.Deserialize<List<int>>(n.LidaPor) ?? new List<int>(),
                    Lida = usuarioId.HasValue && !string.IsNullOrEmpty(n.LidaPor) && 
                           n.LidaPor.Contains(usuarioId.Value.ToString())
                }).ToList();

                // Filtrar apenas não lidas se solicitado
                if (apenasNaoLidas == true && usuarioId.HasValue)
                {
                    notificacoesDto = notificacoesDto.Where(n => !n.Lida).ToList();
                }

                var resultado = new PagedResult<NotificacaoDto>
                {
                    Data = notificacoesDto,
                    TotalCount = totalItens,
                    PageNumber = pagina,
                    PageSize = tamanhoPagina
                };

                return Ok(ApiResponse<PagedResult<NotificacaoDto>>.SuccessResult(resultado));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter notificações");
                throw new BusinessException("Erro ao obter notificações");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<NotificacaoDto>>> ObterNotificacao(int id)
        {
            try
            {
                var notificacao = await _context.Notificacoes.FindAsync(id);

                if (notificacao == null)
                {
                    throw new NotFoundException("Notificação não encontrada");
                }

                var notificacaoDto = new NotificacaoDto
                {
                    Id = notificacao.IdNotificacao,
                    Titulo = notificacao.Titulo,
                    Mensagem = notificacao.Mensagem,
                    Tipo = notificacao.Tipo.ToString().ToLower(),
                    Prioridade = notificacao.Prioridade.ToString().ToLower(),
                    TipoDestino = notificacao.TipoDestino.ToString().ToLower(),
                    IdsDestino = string.IsNullOrEmpty(notificacao.IdsDestino) ? 
                        null : 
                        JsonSerializer.Deserialize<List<int>>(notificacao.IdsDestino),
                    DataCriacao = notificacao.DataCriacao,
                    LidaPor = string.IsNullOrEmpty(notificacao.LidaPor) ? 
                        new List<int>() : 
                        JsonSerializer.Deserialize<List<int>>(notificacao.LidaPor) ?? new List<int>()
                };

                return Ok(ApiResponse<NotificacaoDto>.SuccessResult(notificacaoDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter notificação {Id}", id);
                throw;
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ApiResponse<NotificacaoDto>>> CriarNotificacao([FromBody] CriarNotificacaoDto criarDto)
        {
            try
            {
                // Validar tipo
                if (!Enum.TryParse<TipoNotificacao>(criarDto.Tipo, true, out var tipo))
                {
                    throw new ValidationException(new { Tipo = "Tipo de notificação inválido. Use: info, warning, success, error" });
                }

                // Validar prioridade
                if (!Enum.TryParse<PrioridadeNotificacao>(criarDto.Prioridade, true, out var prioridade))
                {
                    throw new ValidationException(new { Prioridade = "Prioridade inválida. Use: low, normal, high" });
                }

                // Validar tipo de destino
                if (!Enum.TryParse<TipoDestinoNotificacao>(criarDto.TipoDestino, true, out var tipoDestino))
                {
                    throw new ValidationException(new { TipoDestino = "Tipo de destino inválido. Use: all, students, drivers, specific" });
                }

                // Validar IDs de destino se for específico
                if (tipoDestino == TipoDestinoNotificacao.Especifico)
                {
                    if (criarDto.IdsDestino == null || !criarDto.IdsDestino.Any())
                    {
                        throw new ValidationException(new { IdsDestino = "IDs de destino são obrigatórios para notificações específicas" });
                    }

                    // Verificar se os usuários existem
                    var usuariosExistentes = await _context.Usuarios
                        .Where(u => criarDto.IdsDestino.Contains(u.IdUsuario))
                        .CountAsync();

                    var alunosExistentes = await _context.Alunos
                        .Where(a => criarDto.IdsDestino.Contains(a.IdAluno))
                        .CountAsync();

                    if (usuariosExistentes + alunosExistentes != criarDto.IdsDestino.Count)
                    {
                        throw new ValidationException(new { IdsDestino = "Alguns usuários especificados não foram encontrados" });
                    }
                }

                var notificacao = new Notificacao
                {
                    Titulo = criarDto.Titulo,
                    Mensagem = criarDto.Mensagem,
                    Tipo = tipo,
                    Prioridade = prioridade,
                    TipoDestino = tipoDestino,
                    IdsDestino = criarDto.IdsDestino != null ? JsonSerializer.Serialize(criarDto.IdsDestino) : null,
                    DataCriacao = DateTime.Now,
                    LidaPor = "[]"
                };

                _context.Notificacoes.Add(notificacao);
                await _context.SaveChangesAsync();

                var notificacaoDto = new NotificacaoDto
                {
                    Id = notificacao.IdNotificacao,
                    Titulo = notificacao.Titulo,
                    Mensagem = notificacao.Mensagem,
                    Tipo = notificacao.Tipo.ToString().ToLower(),
                    Prioridade = notificacao.Prioridade.ToString().ToLower(),
                    TipoDestino = notificacao.TipoDestino.ToString().ToLower(),
                    IdsDestino = criarDto.IdsDestino,
                    DataCriacao = notificacao.DataCriacao,
                    LidaPor = new List<int>()
                };

                _logger.LogInformation("Notificação criada: {Id} - {Titulo}", notificacao.IdNotificacao, criarDto.Titulo);

                return CreatedAtAction(nameof(ObterNotificacao), new { id = notificacao.IdNotificacao }, 
                    ApiResponse<NotificacaoDto>.SuccessResult(notificacaoDto, "Notificação criada com sucesso"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar notificação");
                throw;
            }
        }

        [HttpPatch("{id}/marcar-como-lida")]
        public async Task<ActionResult<ApiResponse<string>>> MarcarComoLida(int id, [FromBody] MarcarNotificacaoComoLidaDto marcarDto)
        {
            try
            {
                var notificacao = await _context.Notificacoes.FindAsync(id);

                if (notificacao == null)
                {
                    throw new NotFoundException("Notificação não encontrada");
                }

                // Verificar se o usuário tem acesso a esta notificação
                var temAcesso = notificacao.TipoDestino == TipoDestinoNotificacao.Todos ||
                               (notificacao.TipoDestino == TipoDestinoNotificacao.Especifico &&
                                !string.IsNullOrEmpty(notificacao.IdsDestino) &&
                                notificacao.IdsDestino.Contains(marcarDto.UsuarioId.ToString()));

                if (!temAcesso)
                {
                    throw new BusinessException("Usuário não tem acesso a esta notificação");
                }

                // Atualizar lista de usuários que leram
                var lidaPor = string.IsNullOrEmpty(notificacao.LidaPor) ? 
                    new List<int>() : 
                    JsonSerializer.Deserialize<List<int>>(notificacao.LidaPor) ?? new List<int>();

                if (!lidaPor.Contains(marcarDto.UsuarioId))
                {
                    lidaPor.Add(marcarDto.UsuarioId);
                    notificacao.LidaPor = JsonSerializer.Serialize(lidaPor);
                    await _context.SaveChangesAsync();
                }

                _logger.LogInformation("Notificação {Id} marcada como lida pelo usuário {UsuarioId}", id, marcarDto.UsuarioId);

                return Ok(ApiResponse<string>.SuccessResult("Notificação marcada como lida"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao marcar notificação como lida {Id}", id);
                throw;
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ApiResponse<string>>> ExcluirNotificacao(int id)
        {
            try
            {
                var notificacao = await _context.Notificacoes.FindAsync(id);

                if (notificacao == null)
                {
                    throw new NotFoundException("Notificação não encontrada");
                }

                _context.Notificacoes.Remove(notificacao);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Notificação excluída: {Id}", id);

                return Ok(ApiResponse<string>.SuccessResult("Notificação excluída com sucesso"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir notificação {Id}", id);
                throw;
            }
        }

        [HttpGet("nao-lidas/contador")]
        public async Task<ActionResult<ApiResponse<object>>> ContarNotificacoesNaoLidas([FromQuery] int usuarioId)
        {
            try
            {
                var notificacoesNaoLidas = await _context.Notificacoes
                    .Where(n => 
                        (n.TipoDestino == TipoDestinoNotificacao.Todos ||
                         (n.TipoDestino == TipoDestinoNotificacao.Especifico && 
                          !string.IsNullOrEmpty(n.IdsDestino) && 
                          n.IdsDestino.Contains(usuarioId.ToString()))) &&
                        (string.IsNullOrEmpty(n.LidaPor) || !n.LidaPor.Contains(usuarioId.ToString())))
                    .CountAsync();

                var resultado = new { contador = notificacoesNaoLidas };

                return Ok(ApiResponse<object>.SuccessResult(resultado));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar notificações não lidas para usuário {UsuarioId}", usuarioId);
                throw new BusinessException("Erro ao contar notificações não lidas");
            }
        }

        [HttpPost("broadcast")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ApiResponse<string>>> EnviarNotificacaoBroadcast([FromBody] CriarNotificacaoDto criarDto)
        {
            try
            {
                // Forçar tipo de destino para todos
                criarDto.TipoDestino = "all";
                criarDto.IdsDestino = null;

                var resultado = await CriarNotificacao(criarDto);

                _logger.LogInformation("Notificação broadcast enviada: {Titulo}", criarDto.Titulo);

                return Ok(ApiResponse<string>.SuccessResult("Notificação enviada para todos os usuários"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar notificação broadcast");
                throw;
            }
        }
    }
}
