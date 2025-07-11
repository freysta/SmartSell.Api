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

                var stats = new
                {
                    totalStudents = totalStudents,
                    totalDrivers = totalDrivers,
                    totalRoutes = totalRoutes,
                    pendingPayments = 15, // Valor simulado
                    monthlyRevenue = 24800.0, // Valor simulado
                    activeRoutes = activeRoutes
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
