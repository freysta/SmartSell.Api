using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartSell.Api.Data;
using SmartSell.Api.Models.Galdino;

namespace SmartSell.Api.Controllers.Galdino
{
    [ApiController]
    [Route("api/dashboard")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly GaldinoDbContext _context;

        public DashboardController(GaldinoDbContext context)
        {
            _context = context;
        }

        [HttpGet("stats")]
        public async Task<ActionResult> GetDashboardStats()
        {
            try
            {
                var totalStudents = await _context.Alunos.CountAsync();
                var totalDrivers = await _context.Usuarios
                    .Where(u => u.Tipo == Models.Galdino.TipoUsuario.Motorista)
                    .CountAsync();
                
                var today = DateTime.Today;
                var totalRoutes = await _context.Rotas.CountAsync();
                var activeRoutes = await _context.Rotas
                    .Where(r => r.DataRota >= today && r.Status != Models.Galdino.StatusRota.Cancelada)
                    .CountAsync();

                var pendingPayments = await _context.Pagamentos
                    .Where(p => p.Status == StatusPagamento.Pendente || 
                               p.Status == StatusPagamento.Atrasado)
                    .CountAsync();

                var monthlyRevenue = await _context.Pagamentos
                    .Where(p => p.DataPagamento.Month == DateTime.Now.Month && 
                               p.DataPagamento.Year == DateTime.Now.Year &&
                               p.Status == StatusPagamento.Pago)
                    .SumAsync(p => p.Valor);

                var stats = new
                {
                    totalStudents,
                    totalDrivers,
                    totalRoutes,
                    pendingPayments,
                    monthlyRevenue,
                    activeRoutes
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        [HttpGet("recent-activities")]
        public async Task<ActionResult> GetRecentActivities()
        {
            try
            {
                var recentRoutes = await _context.Rotas
                    .Include(r => r.Motorista)
                    .OrderByDescending(r => r.DataRota)
                    .Take(5)
                    .Select(r => new
                    {
                        id = r.IdRota,
                        type = "route",
                        title = $"Rota para {r.Destino}",
                        description = $"Motorista: {r.Motorista.Nome}",
                        date = r.DataRota,
                        status = r.Status.ToString()
                    })
                    .ToListAsync();

                var recentPayments = await _context.Pagamentos
                    .Include(p => p.Aluno)
                    .Where(p => p.Status == StatusPagamento.Pago)
                    .OrderByDescending(p => p.DataPagamento)
                    .Take(5)
                    .Select(p => new
                    {
                        id = p.IdPagamento,
                        type = "payment",
                        title = $"Pagamento recebido",
                        description = $"Aluno: {p.Aluno.Nome} - R$ {p.Valor:F2}",
                        date = p.DataPagamento,
                        status = "completed"
                    })
                    .ToListAsync();

                var activities = recentRoutes.Cast<object>()
                    .Concat(recentPayments.Cast<object>())
                    .OrderByDescending(a => ((dynamic)a).date)
                    .Take(10)
                    .ToList();

                return Ok(activities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }
    }
}
