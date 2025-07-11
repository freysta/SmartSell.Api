using Microsoft.AspNetCore.Mvc;
using SmartSell.Api.Data;

namespace SmartSell.Api.Controllers.Galdino
{
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly GaldinoDbContext _context;

        public DashboardController(GaldinoDbContext context)
        {
            _context = context;
        }

        [HttpGet("stats")]
        public IActionResult GetStats()
        {
            try
            {
                var totalStudents = _context.Alunos.Count();
                var totalDrivers = _context.Usuarios.Count(u => u._tipo == "Motorista");
                var totalRoutes = _context.Rotas.Count();
                var activeRoutes = _context.Rotas.Count(r => r._status == "Ativa");

                var pendingPayments = _context.Pagamentos.Count(p => p._status == "pending");
                var monthlyRevenue = _context.Pagamentos
                    .Where(p => p._status == "paid" && p._paymentDate.HasValue && 
                               p._paymentDate.Value.Month == DateTime.Now.Month &&
                               p._paymentDate.Value.Year == DateTime.Now.Year)
                    .Sum(p => (double)p._amount);

                var stats = new
                {
                    totalStudents = totalStudents,
                    totalDrivers = totalDrivers,
                    totalRoutes = totalRoutes,
                    pendingPayments = pendingPayments,
                    monthlyRevenue = monthlyRevenue,
                    activeRoutes = activeRoutes
                };

                return Ok(new { data = stats, message = "Estatísticas obtidas com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
