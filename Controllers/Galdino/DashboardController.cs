using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartSell.Api.Data;

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
        public async Task<ActionResult<object>> GetStats()
        {
            var totalStudents = await _context.Alunos.CountAsync();
            var totalDrivers = await _context.Usuarios
                .CountAsync(u => u.Tipo == Models.Galdino.TipoUsuario.Motorista);
            var totalRoutes = await _context.Rotas.CountAsync();

            var stats = new
            {
                totalStudents,
                totalDrivers,
                totalRoutes,
                pendingPayments = 5,
                monthlyRevenue = 15000.00,
                activeRoutes = totalRoutes
            };

            return Ok(stats);
        }

        [HttpGet("recent-activities")]
        public ActionResult<object> GetRecentActivities()
        {
            var activities = new[]
            {
                new { 
                    id = 1, 
                    type = "student_registered", 
                    message = "Novo aluno cadastrado", 
                    timestamp = DateTime.Now.AddHours(-2) 
                },
                new { 
                    id = 2, 
                    type = "route_completed", 
                    message = "Rota Campus Norte finalizada", 
                    timestamp = DateTime.Now.AddHours(-4) 
                },
                new { 
                    id = 3, 
                    type = "payment_received", 
                    message = "Pagamento recebido", 
                    timestamp = DateTime.Now.AddHours(-6) 
                }
            };

            return Ok(activities);
        }
    }
}
