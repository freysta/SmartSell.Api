using Microsoft.AspNetCore.Mvc;
using SmartSell.Api.DAO;
using SmartSell.Api.Data;
using SmartSell.Api.Models.Galdino;

namespace SmartSell.Api.Controllers.Galdino
{
    [ApiController]
    [Route("api/routes")]
    public class RoutesController : ControllerBase
    {
        private readonly RotaDAO _rotaDAO;
        private readonly UsuarioDAO _usuarioDAO;
        private readonly RotaAlunoDAO _rotaAlunoDAO;

        public RoutesController(GaldinoDbContext context)
        {
            _rotaDAO = new RotaDAO(context);
            _usuarioDAO = new UsuarioDAO(context);
            _rotaAlunoDAO = new RotaAlunoDAO(context);
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] string? status)
        {
            try
            {
                var todasRotas = _rotaDAO.GetAll();

                if (!string.IsNullOrEmpty(status) && Enum.TryParse<StatusRotaEnum>(status, out var statusEnum))
                {
                    todasRotas = todasRotas.Where(r => r._status == statusEnum).ToList();
                }

                var rotas = todasRotas.Select(r => new
                {
                    id = r._id,
                    date = r._dataRota.ToString("yyyy-MM-dd"),
                    destination = r._tipoRota,
                    departureTime = r._horarioSaida.ToString(@"hh\:mm"),
                    status = r._status,
                    driverId = r._motoristaId,
                    driverName = _usuarioDAO.GetById(r._motoristaId)?._nome ?? "N/A",
                    createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
                }).ToList();

                return Ok(new { data = rotas, message = "Rotas listadas com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var rota = _rotaDAO.GetById(id);
                if (rota == null)
                    return NotFound(new { message = "Rota não encontrada" });

                var response = new
                {
                    id = rota._id,
                    date = rota._dataRota.ToString("yyyy-MM-dd"),
                    destination = rota._tipoRota,
                    departureTime = rota._horarioSaida.ToString(@"hh\:mm"),
                    status = rota._status,
                    driverId = rota._motoristaId,
                    driverName = _usuarioDAO.GetById(rota._motoristaId)?._nome ?? "N/A",
                    createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
                };

                return Ok(new { data = response, message = "Rota encontrada" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateRouteRequest request)
        {
            try
            {
                var motorista = _usuarioDAO.GetById(request.DriverId);

                if (motorista == null)
                {
                    return BadRequest(new { message = "Motorista não encontrado" });
                }

                if (!TimeSpan.TryParse(request.DepartureTime, out TimeSpan horarioSaida))
                {
                    return BadRequest(new { message = "Formato de horário inválido. Use HH:mm" });
                }

                if (!DateTime.TryParse(request.Date, out DateTime dataRota))
                {
                    return BadRequest(new { message = "Formato de data inválido. Use yyyy-MM-dd" });
                }

                var rota = new Rota
                {
                    _dataRota = dataRota,
                    _tipoRota = Enum.TryParse<TipoRotaEnum>(request.Destination, out var tipoEnum) ? tipoEnum : TipoRotaEnum.Ida,
                    _horarioSaida = horarioSaida,
                    _status = Enum.TryParse<StatusRotaEnum>(request.Status, out var statusEnum) ? statusEnum : StatusRotaEnum.Planejada,
                    _motoristaId = request.DriverId,
                    _onibusId = 1, // Valor padrão
                    _instituicaoId = 1 // Valor padrão
                };

                _rotaDAO.Create(rota);

                var response = new
                {
                    id = rota._id,
                    date = rota._dataRota.ToString("yyyy-MM-dd"),
                    destination = rota._tipoRota,
                    departureTime = rota._horarioSaida.ToString(@"hh\:mm"),
                    status = rota._status,
                    driverId = rota._motoristaId,
                    driverName = motorista._nome,
                    createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
                };

                return StatusCode(201, new { data = response, message = "Rota criada com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateRouteRequest request)
        {
            try
            {
                var rota = _rotaDAO.GetById(id);
                if (rota == null)
                    return NotFound(new { message = "Rota não encontrada" });

                if (request.DriverId.HasValue)
                {
                    var motorista = _usuarioDAO.GetById(request.DriverId.Value);

                    if (motorista == null)
                    {
                        return BadRequest(new { message = "Motorista não encontrado" });
                    }
                    rota._motoristaId = request.DriverId.Value;
                }

                if (!string.IsNullOrEmpty(request.Date))
                {
                    if (DateTime.TryParse(request.Date, out DateTime dataRota))
                    {
                        rota._dataRota = dataRota;
                    }
                    else
                    {
                        return BadRequest(new { message = "Formato de data inválido. Use yyyy-MM-dd" });
                    }
                }

                if (!string.IsNullOrEmpty(request.DepartureTime))
                {
                    if (TimeSpan.TryParse(request.DepartureTime, out TimeSpan horarioSaida))
                    {
                        rota._horarioSaida = horarioSaida;
                    }
                    else
                    {
                        return BadRequest(new { message = "Formato de horário inválido. Use HH:mm" });
                    }
                }

                if (!string.IsNullOrEmpty(request.Destination) && Enum.TryParse<TipoRotaEnum>(request.Destination, out var tipoEnum))
                    rota._tipoRota = tipoEnum;
                
                if (!string.IsNullOrEmpty(request.Status) && Enum.TryParse<StatusRotaEnum>(request.Status, out var statusEnum))
                    rota._status = statusEnum;

                _rotaDAO.Update(rota);

                var response = new
                {
                    id = rota._id,
                    date = rota._dataRota.ToString("yyyy-MM-dd"),
                    destination = rota._tipoRota,
                    departureTime = rota._horarioSaida.ToString(@"hh\:mm"),
                    status = rota._status,
                    driverId = rota._motoristaId,
                    driverName = _usuarioDAO.GetById(rota._motoristaId)?._nome ?? "N/A",
                    createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
                };

                return Ok(new { data = response, message = "Rota atualizada com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var rota = _rotaDAO.GetById(id);
                if (rota == null)
                    return NotFound(new { message = "Rota não encontrada" });

                var alunosAssociados = _rotaAlunoDAO.GetByRota(id).Any();
                if (alunosAssociados)
                {
                    return BadRequest(new { message = "Não é possível excluir rota com alunos associados" });
                }

                _rotaDAO.Delete(id);
                return Ok(new { message = "Rota removida com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }

    public class CreateRouteRequest
    {
        public string Date { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public string DepartureTime { get; set; } = string.Empty;
        public string? Status { get; set; }
        public int DriverId { get; set; }
    }

    public class UpdateRouteRequest
    {
        public string? Date { get; set; }
        public string? Destination { get; set; }
        public string? DepartureTime { get; set; }
        public string? Status { get; set; }
        public int? DriverId { get; set; }
    }
}
