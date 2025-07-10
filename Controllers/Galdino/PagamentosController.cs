using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartSell.Api.Data;
using SmartSell.Api.DTOs;
using SmartSell.Api.Models.Galdino;
using SmartSell.Api.Middleware;

namespace SmartSell.Api.Controllers.Galdino
{
    [ApiController]
    [Route("api/pagamentos")]
    [Authorize]
    public class PagamentosController : ControllerBase
    {
        private readonly GaldinoDbContext _context;
        private readonly ILogger<PagamentosController> _logger;

        public PagamentosController(GaldinoDbContext context, ILogger<PagamentosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<PagamentoDto>>>> ObterPagamentos(
            [FromQuery] int? alunoId = null,
            [FromQuery] string? status = null,
            [FromQuery] string? mes = null,
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanhoPagina = 10)
        {
            try
            {
                var query = _context.Pagamentos
                    .Include(p => p.Aluno)
                    .AsQueryable();

                // Aplicar filtros
                if (alunoId.HasValue)
                {
                    query = query.Where(p => p.FkIdAluno == alunoId.Value);
                }

                if (!string.IsNullOrEmpty(status))
                {
                    if (Enum.TryParse<StatusPagamento>(status, true, out var statusEnum))
                    {
                        query = query.Where(p => p.Status == statusEnum);
                    }
                }

                if (!string.IsNullOrEmpty(mes))
                {
                    if (DateTime.TryParse($"{mes}-01", out var dataFiltro))
                    {
                        query = query.Where(p => p.DataPagamento.Month == dataFiltro.Month && 
                                               p.DataPagamento.Year == dataFiltro.Year);
                    }
                }

                var totalItens = await query.CountAsync();
                var pagamentos = await query
                    .OrderByDescending(p => p.DataPagamento)
                    .Skip((pagina - 1) * tamanhoPagina)
                    .Take(tamanhoPagina)
                    .ToListAsync();

                var pagamentosDto = pagamentos.Select(p => new PagamentoDto
                {
                    Id = p.IdPagamento,
                    AlunoId = p.FkIdAluno,
                    NomeAluno = p.Aluno.Nome,
                    Valor = p.Valor,
                    Mes = p.DataPagamento.ToString("yyyy-MM"),
                    Ano = p.DataPagamento.Year,
                    Status = p.Status.ToString().ToLower(),
                    MetodoPagamento = p.MetodoPagamento,
                    DataPagamento = p.Status == StatusPagamento.Pago ? p.DataPagamento : null,
                    DataVencimento = p.DataVencimento,
                    DataCriacao = p.DataPagamento
                }).ToList();

                var resultado = new PagedResult<PagamentoDto>
                {
                    Data = pagamentosDto,
                    TotalCount = totalItens,
                    PageNumber = pagina,
                    PageSize = tamanhoPagina
                };

                return Ok(ApiResponse<PagedResult<PagamentoDto>>.SuccessResult(resultado));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter pagamentos");
                throw new BusinessException("Erro ao obter pagamentos");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<PagamentoDto>>> ObterPagamento(int id)
        {
            try
            {
                var pagamento = await _context.Pagamentos
                    .Include(p => p.Aluno)
                    .FirstOrDefaultAsync(p => p.IdPagamento == id);

                if (pagamento == null)
                {
                    throw new NotFoundException("Pagamento não encontrado");
                }

                var pagamentoDto = new PagamentoDto
                {
                    Id = pagamento.IdPagamento,
                    AlunoId = pagamento.FkIdAluno,
                    NomeAluno = pagamento.Aluno.Nome,
                    Valor = pagamento.Valor,
                    Mes = pagamento.DataPagamento.ToString("yyyy-MM"),
                    Ano = pagamento.DataPagamento.Year,
                    Status = pagamento.Status.ToString().ToLower(),
                    MetodoPagamento = pagamento.MetodoPagamento,
                    DataPagamento = pagamento.Status == StatusPagamento.Pago ? pagamento.DataPagamento : null,
                    DataVencimento = pagamento.DataVencimento,
                    DataCriacao = pagamento.DataPagamento
                };

                return Ok(ApiResponse<PagamentoDto>.SuccessResult(pagamentoDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter pagamento {Id}", id);
                throw;
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ApiResponse<PagamentoDto>>> CriarPagamento([FromBody] CriarPagamentoDto criarDto)
        {
            try
            {
                // Verificar se o aluno existe
                var aluno = await _context.Alunos.FindAsync(criarDto.AlunoId);
                if (aluno == null)
                {
                    throw new NotFoundException("Aluno não encontrado");
                }

                // Verificar se já existe pagamento para o mesmo mês/ano
                var mesAno = DateTime.Parse($"{criarDto.Mes}-01");
                var pagamentoExistente = await _context.Pagamentos
                    .FirstOrDefaultAsync(p => p.FkIdAluno == criarDto.AlunoId &&
                                            p.DataPagamento.Month == mesAno.Month &&
                                            p.DataPagamento.Year == mesAno.Year);

                if (pagamentoExistente != null)
                {
                    throw new BusinessException("Já existe um pagamento para este aluno no mês/ano informado");
                }

                // Determinar status baseado na data de vencimento
                var status = criarDto.DataVencimento < DateTime.Now ? StatusPagamento.Atrasado : StatusPagamento.Pendente;

                var pagamento = new Pagamento
                {
                    FkIdAluno = criarDto.AlunoId,
                    Valor = criarDto.Valor,
                    DataPagamento = mesAno,
                    DataVencimento = criarDto.DataVencimento,
                    Status = status,
                    MetodoPagamento = criarDto.MetodoPagamento
                };

                _context.Pagamentos.Add(pagamento);
                await _context.SaveChangesAsync();

                var pagamentoDto = new PagamentoDto
                {
                    Id = pagamento.IdPagamento,
                    AlunoId = pagamento.FkIdAluno,
                    NomeAluno = aluno.Nome,
                    Valor = pagamento.Valor,
                    Mes = pagamento.DataPagamento.ToString("yyyy-MM"),
                    Ano = pagamento.DataPagamento.Year,
                    Status = pagamento.Status.ToString().ToLower(),
                    MetodoPagamento = pagamento.MetodoPagamento,
                    DataPagamento = null,
                    DataVencimento = pagamento.DataVencimento,
                    DataCriacao = DateTime.Now
                };

                _logger.LogInformation("Pagamento criado: {Id} para aluno {AlunoId}", pagamento.IdPagamento, criarDto.AlunoId);

                return CreatedAtAction(nameof(ObterPagamento), new { id = pagamento.IdPagamento }, 
                    ApiResponse<PagamentoDto>.SuccessResult(pagamentoDto, "Pagamento criado com sucesso"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar pagamento para aluno {AlunoId}", criarDto.AlunoId);
                throw;
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ApiResponse<string>>> AtualizarPagamento(int id, [FromBody] AtualizarPagamentoDto atualizarDto)
        {
            try
            {
                var pagamento = await _context.Pagamentos.FindAsync(id);
                if (pagamento == null)
                {
                    throw new NotFoundException("Pagamento não encontrado");
                }

                // Não permitir edição de pagamentos já quitados
                if (pagamento.Status == StatusPagamento.Pago)
                {
                    throw new BusinessException("Não é possível editar pagamentos já quitados");
                }

                // Atualizar campos se fornecidos
                if (atualizarDto.Valor.HasValue)
                    pagamento.Valor = atualizarDto.Valor.Value;

                if (!string.IsNullOrEmpty(atualizarDto.Mes))
                {
                    var novaData = DateTime.Parse($"{atualizarDto.Mes}-01");
                    pagamento.DataPagamento = novaData;
                }

                if (atualizarDto.DataVencimento.HasValue)
                    pagamento.DataVencimento = atualizarDto.DataVencimento.Value;

                if (!string.IsNullOrEmpty(atualizarDto.MetodoPagamento))
                    pagamento.MetodoPagamento = atualizarDto.MetodoPagamento;

                if (!string.IsNullOrEmpty(atualizarDto.Status))
                {
                    if (Enum.TryParse<StatusPagamento>(atualizarDto.Status, true, out var novoStatus))
                    {
                        pagamento.Status = novoStatus;
                    }
                }

                // Atualizar status baseado na data de vencimento se não foi explicitamente definido
                if (string.IsNullOrEmpty(atualizarDto.Status) && pagamento.Status != StatusPagamento.Pago)
                {
                    pagamento.Status = pagamento.DataVencimento < DateTime.Now ? StatusPagamento.Atrasado : StatusPagamento.Pendente;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Pagamento atualizado: {Id}", id);

                return Ok(ApiResponse<string>.SuccessResult("Pagamento atualizado com sucesso"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar pagamento {Id}", id);
                throw;
            }
        }

        [HttpPatch("{id}/marcar-como-quitado")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ApiResponse<PagamentoDto>>> MarcarComoQuitado(int id, [FromBody] MarcarPagamentoComoQuitadoDto marcarDto)
        {
            try
            {
                var pagamento = await _context.Pagamentos
                    .Include(p => p.Aluno)
                    .FirstOrDefaultAsync(p => p.IdPagamento == id);

                if (pagamento == null)
                {
                    throw new NotFoundException("Pagamento não encontrado");
                }

                if (pagamento.Status == StatusPagamento.Pago)
                {
                    throw new BusinessException("Pagamento já está quitado");
                }

                pagamento.Status = StatusPagamento.Pago;
                pagamento.MetodoPagamento = marcarDto.MetodoPagamento;
                pagamento.DataPagamento = marcarDto.DataPagamento ?? DateTime.Now;

                await _context.SaveChangesAsync();

                var pagamentoDto = new PagamentoDto
                {
                    Id = pagamento.IdPagamento,
                    AlunoId = pagamento.FkIdAluno,
                    NomeAluno = pagamento.Aluno.Nome,
                    Valor = pagamento.Valor,
                    Mes = pagamento.DataPagamento.ToString("yyyy-MM"),
                    Ano = pagamento.DataPagamento.Year,
                    Status = pagamento.Status.ToString().ToLower(),
                    MetodoPagamento = pagamento.MetodoPagamento,
                    DataPagamento = pagamento.DataPagamento,
                    DataVencimento = pagamento.DataVencimento,
                    DataCriacao = pagamento.DataPagamento
                };

                _logger.LogInformation("Pagamento marcado como quitado: {Id}", id);

                return Ok(ApiResponse<PagamentoDto>.SuccessResult(pagamentoDto, "Pagamento marcado como quitado"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao marcar pagamento como quitado {Id}", id);
                throw;
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ApiResponse<string>>> ExcluirPagamento(int id)
        {
            try
            {
                var pagamento = await _context.Pagamentos.FindAsync(id);
                if (pagamento == null)
                {
                    throw new NotFoundException("Pagamento não encontrado");
                }

                // Não permitir exclusão de pagamentos quitados
                if (pagamento.Status == StatusPagamento.Pago)
                {
                    throw new BusinessException("Não é possível excluir pagamentos quitados");
                }

                _context.Pagamentos.Remove(pagamento);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Pagamento excluído: {Id}", id);

                return Ok(ApiResponse<string>.SuccessResult("Pagamento excluído com sucesso"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir pagamento {Id}", id);
                throw;
            }
        }

        [HttpGet("relatorio-mensal")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ApiResponse<object>>> ObterRelatorioMensal([FromQuery] int ano, [FromQuery] int mes)
        {
            try
            {
                var pagamentos = await _context.Pagamentos
                    .Include(p => p.Aluno)
                    .Where(p => p.DataPagamento.Year == ano && p.DataPagamento.Month == mes)
                    .ToListAsync();

                var relatorio = new
                {
                    Periodo = $"{mes:D2}/{ano}",
                    TotalPagamentos = pagamentos.Count,
                    PagamentosQuitados = pagamentos.Count(p => p.Status == StatusPagamento.Pago),
                    PagamentosPendentes = pagamentos.Count(p => p.Status == StatusPagamento.Pendente),
                    PagamentosAtrasados = pagamentos.Count(p => p.Status == StatusPagamento.Atrasado),
                    ValorTotal = pagamentos.Sum(p => p.Valor),
                    ValorRecebido = pagamentos.Where(p => p.Status == StatusPagamento.Pago).Sum(p => p.Valor),
                    ValorPendente = pagamentos.Where(p => p.Status != StatusPagamento.Pago).Sum(p => p.Valor)
                };

                return Ok(ApiResponse<object>.SuccessResult(relatorio));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar relatório mensal {Ano}/{Mes}", ano, mes);
                throw new BusinessException("Erro ao gerar relatório mensal");
            }
        }
    }
}
