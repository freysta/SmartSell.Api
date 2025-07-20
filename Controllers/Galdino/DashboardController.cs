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
        private readonly MotoristaDAO _motoristaDAO;
        private readonly RotaDAO _rotaDAO;
        private readonly PagamentoDAO _pagamentoDAO;

        public DashboardController(GaldinoDbContext context)
        {
            _alunoDAO = new AlunoDAO(context);
            _motoristaDAO = new MotoristaDAO(context);
            _rotaDAO = new RotaDAO(context);
            _pagamentoDAO = new PagamentoDAO(context);
        }

        [HttpGet("stats")]
        public IActionResult GetStats()
        {
            try
            {
                int totalStudents = 0;
                int totalDrivers = 0;
                int totalRoutes = 0;
                int activeRoutes = 0;
                int pendingPayments = 0;
                double monthlyRevenue = 0;

                try
                {
                    totalStudents = _alunoDAO.GetAll().Count;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao contar estudantes: {ex.Message}");
                    totalStudents = 0;
                }

                try
                {
                    totalDrivers = _motoristaDAO.GetAll().Count;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao contar motoristas: {ex.Message}");
                    totalDrivers = 0;
                }

                try
                {
                    var rotas = _rotaDAO.GetAll();
                    totalRoutes = rotas.Count;
                    activeRoutes = rotas.Count(r => r._status == StatusRotaEnum.Planejada || r._status == StatusRotaEnum.EmAndamento);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao contar rotas: {ex.Message}");
                    totalRoutes = 0;
                    activeRoutes = 0;
                }

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
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao processar pagamentos: {ex.Message}");
                    pendingPayments = 0;
                    monthlyRevenue = 0;
                }

                var stats = new
                {
                    totalStudents = totalStudents,
                    totalDrivers = totalDrivers,
                    totalRoutes = totalRoutes,
                    activeRoutes = activeRoutes,
                    pendingPayments = pendingPayments,
                    monthlyRevenue = monthlyRevenue,
                    lastUpdated = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };

                return Ok(new { 
                    success = true,
                    data = stats, 
                    message = "Estatísticas obtidas com sucesso" 
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro geral no dashboard: {ex.Message}");
                return StatusCode(500, new { 
                    success = false,
                    message = $"Erro interno do servidor: {ex.Message}",
                    data = new {
                        totalStudents = 0,
                        totalDrivers = 0,
                        totalRoutes = 0,
                        activeRoutes = 0,
                        pendingPayments = 0,
                        monthlyRevenue = 0,
                        lastUpdated = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    }
                });
            }
        }
    }
}
