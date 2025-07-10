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
    [Route("api/pontos-embarque")]
    [Authorize]
    public class PontosEmbarqueController : ControllerBase
    {
        private readonly GaldinoDbContext _context;
        private readonly ILogger<PontosEmbarqueController> _logger;

        public PontosEmbarqueController(GaldinoDbContext context, ILogger<PontosEmbarqueController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<PontoEmbarqueDto>>>> ObterPontosEmbarque(
            [FromQuery] string? cidade = null,
            [FromQuery] string? bairro = null,
            [FromQuery] string? status = null,
            [FromQuery] int? rotaId = null,
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanhoPagina = 10)
        {
            try
            {
                var query = _context.PontosEmbarque.AsQueryable();

                // Aplicar filtros
                if (!string.IsNullOrEmpty(cidade))
                {
                    query = query.Where(p => p.Cidade.Contains(cidade));
                }

                if (!string.IsNullOrEmpty(bairro))
                {
                    query = query.Where(p => p.Bairro.Contains(bairro));
                }

                if (!string.IsNullOrEmpty(status))
                {
                    if (Enum.TryParse<StatusPontoEmbarque>(status, true, out var statusEnum))
                    {
                        query = query.Where(p => p.Status == statusEnum);
                    }
                }

                if (rotaId.HasValue)
                {
                    query = query.Where(p => !string.IsNullOrEmpty(p.RotasIds) && 
                                           p.RotasIds.Contains(rotaId.Value.ToString()));
                }

                var totalItens = await query.CountAsync();
                var pontosEmbarque = await query
                    .OrderBy(p => p.Cidade)
                    .ThenBy(p => p.Nome)
                    .Skip((pagina - 1) * tamanhoPagina)
                    .Take(tamanhoPagina)
                    .ToListAsync();

                var pontosEmbarqueDto = pontosEmbarque.Select(p => new PontoEmbarqueDto
                {
                    Id = p.IdPontoEmbarque,
                    Nome = p.Nome,
                    Endereco = p.Endereco,
                    Bairro = p.Bairro,
                    Cidade = p.Cidade,
                    Coordenadas = p.Latitude.HasValue && p.Longitude.HasValue ? 
                        new CoordenadaDto { Latitude = p.Latitude.Value, Longitude = p.Longitude.Value } : 
                        null,
                    Status = p.Status.ToString().ToLower(),
                    Rotas = string.IsNullOrEmpty(p.RotasIds) ? 
                        new List<int>() : 
                        JsonSerializer.Deserialize<List<int>>(p.RotasIds) ?? new List<int>(),
                    DataCriacao = DateTime.Now // Assumindo que não temos este campo no modelo atual
                }).ToList();

                var resultado = new PagedResult<PontoEmbarqueDto>
                {
                    Data = pontosEmbarqueDto,
                    TotalCount = totalItens,
                    PageNumber = pagina,
                    PageSize = tamanhoPagina
                };

                return Ok(ApiResponse<PagedResult<PontoEmbarqueDto>>.SuccessResult(resultado));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter pontos de embarque");
                throw new BusinessException("Erro ao obter pontos de embarque");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<PontoEmbarqueDto>>> ObterPontoEmbarque(int id)
        {
            try
            {
                var pontoEmbarque = await _context.PontosEmbarque.FindAsync(id);

                if (pontoEmbarque == null)
                {
                    throw new NotFoundException("Ponto de embarque não encontrado");
                }

                var pontoEmbarqueDto = new PontoEmbarqueDto
                {
                    Id = pontoEmbarque.IdPontoEmbarque,
                    Nome = pontoEmbarque.Nome,
                    Endereco = pontoEmbarque.Endereco,
                    Bairro = pontoEmbarque.Bairro,
                    Cidade = pontoEmbarque.Cidade,
                    Coordenadas = pontoEmbarque.Latitude.HasValue && pontoEmbarque.Longitude.HasValue ? 
                        new CoordenadaDto { Latitude = pontoEmbarque.Latitude.Value, Longitude = pontoEmbarque.Longitude.Value } : 
                        null,
                    Status = pontoEmbarque.Status.ToString().ToLower(),
                    Rotas = string.IsNullOrEmpty(pontoEmbarque.RotasIds) ? 
                        new List<int>() : 
                        JsonSerializer.Deserialize<List<int>>(pontoEmbarque.RotasIds) ?? new List<int>(),
                    DataCriacao = DateTime.Now
                };

                return Ok(ApiResponse<PontoEmbarqueDto>.SuccessResult(pontoEmbarqueDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter ponto de embarque {Id}", id);
                throw;
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ApiResponse<PontoEmbarqueDto>>> CriarPontoEmbarque([FromBody] CriarPontoEmbarqueDto criarDto)
        {
            try
            {
                // Verificar se já existe um ponto com o mesmo nome na mesma cidade
                var pontoExistente = await _context.PontosEmbarque
                    .FirstOrDefaultAsync(p => p.Nome == criarDto.Nome && p.Cidade == criarDto.Cidade);

                if (pontoExistente != null)
                {
                    throw new BusinessException("Já existe um ponto de embarque com este nome nesta cidade");
                }

                // Verificar se as rotas existem
                if (criarDto.RotasIds != null && criarDto.RotasIds.Any())
                {
                    var rotasExistentes = await _context.Rotas
                        .Where(r => criarDto.RotasIds.Contains(r.IdRota))
                        .CountAsync();

                    if (rotasExistentes != criarDto.RotasIds.Count)
                    {
                        throw new ValidationException(new { RotasIds = "Algumas rotas especificadas não foram encontradas" });
                    }
                }

                var pontoEmbarque = new PontoEmbarque
                {
                    Nome = criarDto.Nome,
                    Endereco = criarDto.Endereco,
                    Bairro = criarDto.Bairro,
                    Cidade = criarDto.Cidade,
                    Latitude = criarDto.Latitude,
                    Longitude = criarDto.Longitude,
                    Status = StatusPontoEmbarque.Ativo,
                    RotasIds = criarDto.RotasIds != null ? JsonSerializer.Serialize(criarDto.RotasIds) : "[]"
                };

                _context.PontosEmbarque.Add(pontoEmbarque);
                await _context.SaveChangesAsync();

                var pontoEmbarqueDto = new PontoEmbarqueDto
                {
                    Id = pontoEmbarque.IdPontoEmbarque,
                    Nome = pontoEmbarque.Nome,
                    Endereco = pontoEmbarque.Endereco,
                    Bairro = pontoEmbarque.Bairro,
                    Cidade = pontoEmbarque.Cidade,
                    Coordenadas = pontoEmbarque.Latitude.HasValue && pontoEmbarque.Longitude.HasValue ? 
                        new CoordenadaDto { Latitude = pontoEmbarque.Latitude.Value, Longitude = pontoEmbarque.Longitude.Value } : 
                        null,
                    Status = pontoEmbarque.Status.ToString().ToLower(),
                    Rotas = criarDto.RotasIds ?? new List<int>(),
                    DataCriacao = DateTime.Now
                };

                _logger.LogInformation("Ponto de embarque criado: {Id} - {Nome}", pontoEmbarque.IdPontoEmbarque, criarDto.Nome);

                return CreatedAtAction(nameof(ObterPontoEmbarque), new { id = pontoEmbarque.IdPontoEmbarque }, 
                    ApiResponse<PontoEmbarqueDto>.SuccessResult(pontoEmbarqueDto, "Ponto de embarque criado com sucesso"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar ponto de embarque");
                throw;
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ApiResponse<string>>> AtualizarPontoEmbarque(int id, [FromBody] AtualizarPontoEmbarqueDto atualizarDto)
        {
            try
            {
                var pontoEmbarque = await _context.PontosEmbarque.FindAsync(id);
                if (pontoEmbarque == null)
                {
                    throw new NotFoundException("Ponto de embarque não encontrado");
                }

                // Verificar se o novo nome já existe (se fornecido)
                if (!string.IsNullOrEmpty(atualizarDto.Nome) && atualizarDto.Nome != pontoEmbarque.Nome)
                {
                    var cidade = atualizarDto.Cidade ?? pontoEmbarque.Cidade;
                    var pontoExistente = await _context.PontosEmbarque
                        .FirstOrDefaultAsync(p => p.Nome == atualizarDto.Nome && p.Cidade == cidade && p.IdPontoEmbarque != id);

                    if (pontoExistente != null)
                    {
                        throw new BusinessException("Já existe um ponto de embarque com este nome nesta cidade");
                    }
                }

                // Verificar se as rotas existem (se fornecidas)
                if (atualizarDto.RotasIds != null && atualizarDto.RotasIds.Any())
                {
                    var rotasExistentes = await _context.Rotas
                        .Where(r => atualizarDto.RotasIds.Contains(r.IdRota))
                        .CountAsync();

                    if (rotasExistentes != atualizarDto.RotasIds.Count)
                    {
                        throw new ValidationException(new { RotasIds = "Algumas rotas especificadas não foram encontradas" });
                    }
                }

                // Atualizar campos se fornecidos
                if (!string.IsNullOrEmpty(atualizarDto.Nome))
                    pontoEmbarque.Nome = atualizarDto.Nome;

                if (!string.IsNullOrEmpty(atualizarDto.Endereco))
                    pontoEmbarque.Endereco = atualizarDto.Endereco;

                if (!string.IsNullOrEmpty(atualizarDto.Bairro))
                    pontoEmbarque.Bairro = atualizarDto.Bairro;

                if (!string.IsNullOrEmpty(atualizarDto.Cidade))
                    pontoEmbarque.Cidade = atualizarDto.Cidade;

                if (atualizarDto.Latitude.HasValue)
                    pontoEmbarque.Latitude = atualizarDto.Latitude;

                if (atualizarDto.Longitude.HasValue)
                    pontoEmbarque.Longitude = atualizarDto.Longitude;

                if (!string.IsNullOrEmpty(atualizarDto.Status))
                {
                    if (Enum.TryParse<StatusPontoEmbarque>(atualizarDto.Status, true, out var novoStatus))
                    {
                        pontoEmbarque.Status = novoStatus;
                    }
                }

                if (atualizarDto.RotasIds != null)
                {
                    pontoEmbarque.RotasIds = JsonSerializer.Serialize(atualizarDto.RotasIds);
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Ponto de embarque atualizado: {Id}", id);

                return Ok(ApiResponse<string>.SuccessResult("Ponto de embarque atualizado com sucesso"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar ponto de embarque {Id}", id);
                throw;
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ApiResponse<string>>> ExcluirPontoEmbarque(int id)
        {
            try
            {
                var pontoEmbarque = await _context.PontosEmbarque.FindAsync(id);
                if (pontoEmbarque == null)
                {
                    throw new NotFoundException("Ponto de embarque não encontrado");
                }

                // Verificar se há rotas associadas
                if (!string.IsNullOrEmpty(pontoEmbarque.RotasIds))
                {
                    var rotas = JsonSerializer.Deserialize<List<int>>(pontoEmbarque.RotasIds);
                    if (rotas != null && rotas.Any())
                    {
                        throw new BusinessException("Não é possível excluir ponto de embarque que possui rotas associadas");
                    }
                }

                _context.PontosEmbarque.Remove(pontoEmbarque);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Ponto de embarque excluído: {Id}", id);

                return Ok(ApiResponse<string>.SuccessResult("Ponto de embarque excluído com sucesso"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir ponto de embarque {Id}", id);
                throw;
            }
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ApiResponse<string>>> AlterarStatus(int id, [FromBody] string novoStatus)
        {
            try
            {
                var pontoEmbarque = await _context.PontosEmbarque.FindAsync(id);
                if (pontoEmbarque == null)
                {
                    throw new NotFoundException("Ponto de embarque não encontrado");
                }

                if (!Enum.TryParse<StatusPontoEmbarque>(novoStatus, true, out var status))
                {
                    throw new ValidationException(new { Status = "Status inválido. Use: ativo, inativo, manutencao" });
                }

                pontoEmbarque.Status = status;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Status do ponto de embarque alterado: {Id} para {Status}", id, novoStatus);

                return Ok(ApiResponse<string>.SuccessResult($"Status alterado para {novoStatus.ToLower()}"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao alterar status do ponto de embarque {Id}", id);
                throw;
            }
        }

        [HttpGet("por-cidade")]
        public async Task<ActionResult<ApiResponse<object>>> ObterPontosPorCidade()
        {
            try
            {
                var pontosPorCidade = await _context.PontosEmbarque
                    .Where(p => p.Status == StatusPontoEmbarque.Ativo)
                    .GroupBy(p => p.Cidade)
                    .Select(g => new
                    {
                        Cidade = g.Key,
                        TotalPontos = g.Count(),
                        Pontos = g.Select(p => new
                        {
                            Id = p.IdPontoEmbarque,
                            Nome = p.Nome,
                            Bairro = p.Bairro,
                            Endereco = p.Endereco
                        }).ToList()
                    })
                    .OrderBy(x => x.Cidade)
                    .ToListAsync();

                return Ok(ApiResponse<object>.SuccessResult(pontosPorCidade));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter pontos por cidade");
                throw new BusinessException("Erro ao obter pontos por cidade");
            }
        }
    }
}
