using Microsoft.AspNetCore.Mvc;
using SmartSell.Api.DAO;
using SmartSell.Api.Data;
using SmartSell.Api.Models.Galdino;

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
                
                int activeRoutes = 0;
                try
                {
                    var rotas = _rotaDAO.GetAll();
                    activeRoutes = rotas.Count(r => r._status == StatusRotaEnum.Planejada || r._status == StatusRotaEnum.EmAndamento);
                }
                catch (Exception)
                {
                    activeRoutes = totalRoutes;
                }

                int pendingPayments = 0;
                double monthlyRevenue = 0;
                try
                {
                    var pagamentos = _pagamentoDAO.GetAll();
                    pendingPayments = pagamentos.Count(p => p._status == StatusPagamentoEnum.Pendente);
                    monthlyRevenue = pagamentos
                        .Where(p => p._status == StatusPagamentoEnum.Pago && 
                                   p._dataPagamento.Month == DateTime.Now.Month &&
                                   p._dataPagamento.Year == DateTime.Now.Year)
                        .Sum(p => (double)p._valor);
                }
                catch (Exception)
                {
                    pendingPayments = 0;
                    monthlyRevenue = 0;
                }

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
