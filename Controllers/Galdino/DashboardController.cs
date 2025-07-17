using Microsoft.AspNetCore.Mvc;
using SmartSell.Api.DAO;
using SmartSell.Api.Data;

namespace SmartSell.Api.Controllers.Galdino
{
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly AlunoDAO _alunoDAO;
        private readonly UsuarioDAO _usuarioDAO;
        private readonly RotaDAO _rotaDAO;
        private readonly PagamentoDAO _pagamentoDAO;

        public DashboardController(GaldinoDbContext context)
        {
            _alunoDAO = new AlunoDAO(context);
            _usuarioDAO = new UsuarioDAO(context);
            _rotaDAO = new RotaDAO(context);
            _pagamentoDAO = new PagamentoDAO(context);
        }

        [HttpGet("stats")]
        public IActionResult GetStats()
        {
            try
            {
                var totalStudents = _alunoDAO.GetAll().Count;
                var totalDrivers = _usuarioDAO.GetAll().Count;
                var totalRoutes = _rotaDAO.GetAll().Count;
                var activeRoutes = _rotaDAO.GetAll().Count(r => r._status == "Ativa");

                var pendingPayments = _pagamentoDAO.GetAll().Count(p => p._status == "Pendente");
                var monthlyRevenue = _pagamentoDAO.GetAll()
                    .Where(p => p._status == "Pago" && 
                               p._dataPagamento.Month == DateTime.Now.Month &&
                               p._dataPagamento.Year == DateTime.Now.Year)
                    .Sum(p => (double)p._valor);

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
