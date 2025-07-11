using Microsoft.AspNetCore.Mvc;
using SmartSell.Api.Data;
using SmartSell.Api.Models.Galdino;

namespace SmartSell.Api.Controllers.Galdino
{
    [ApiController]
    [Route("api/routes")]
    public class RoutesController : ControllerBase
    {
        private readonly GaldinoDbContext _context;

        public RoutesController(GaldinoDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] string? status)
        {
            try
            {
                var query = _context.Rotas.AsQueryable();

                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(r => r._status == status);
                }

                var rotas = query.Select(r => new
                {
                    id = r._id,
                    date = r._dataRota.ToString("yyyy-MM-dd"),
                    destination = r._destino,
                    departureTime = r._horarioSaida.ToString(@"hh\:mm"),
                    status = r._status,
                    driverId = r._fkIdMotorista,
                    driverName = _context.Usuarios
                        .Where(u => u._id == r._fkIdMotorista)
                        .Select(u => u._nome)
                        .FirstOrDefault(),
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
                var rota = _context.Rotas.Find(id);
                if (rota == null)
                    return NotFound(new { message = "Rota não encontrada" });

                var response = new
                {
                    id = rota._id,
                    date = rota._dataRota.ToString("yyyy-MM-dd"),
                    destination = rota._destino,
                    departureTime = rota._horarioSaida.ToString(@"hh\:mm"),
                    status = rota._status,
                    driverId = rota._fkIdMotorista,
                    driverName = _context.Usuarios
                        .Where(u => u._id == rota._fkIdMotorista)
                        .Select(u => u._nome)
                        .FirstOrDefault(),
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
                var motorista = _context.Usuarios
                    .FirstOrDefault(u => u._id == request.DriverId && u._tipo == "Motorista");

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
                    _destino = request.Destination,
                    _horarioSaida = horarioSaida,
                    _status = request.Status ?? "Ativa",
                    _fkIdMotorista = request.DriverId
                };

                _context.Rotas.Add(rota);
                _context.SaveChanges();

                var response = new
                {
                    id = rota._id,
                    date = rota._dataRota.ToString("yyyy-MM-dd"),
                    destination = rota._destino,
                    departureTime = rota._horarioSaida.ToString(@"hh\:mm"),
                    status = rota._status,
                    driverId = rota._fkIdMotorista,
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
                var rota = _context.Rotas.Find(id);
                if (rota == null)
                    return NotFound(new { message = "Rota não encontrada" });

                if (request.DriverId.HasValue)
                {
                    var motorista = _context.Usuarios
                        .FirstOrDefault(u => u._id == request.DriverId && u._tipo == "Motorista");

                    if (motorista == null)
                    {
                        return BadRequest(new { message = "Motorista não encontrado" });
                    }
                    rota._fkIdMotorista = request.DriverId.Value;
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

                rota._destino = request.Destination ?? rota._destino;
                rota._status = request.Status ?? rota._status;

                _context.SaveChanges();

                var response = new
                {
                    id = rota._id,
                    date = rota._dataRota.ToString("yyyy-MM-dd"),
                    destination = rota._destino,
                    departureTime = rota._horarioSaida.ToString(@"hh\:mm"),
                    status = rota._status,
                    driverId = rota._fkIdMotorista,
                    driverName = _context.Usuarios
                        .Where(u => u._id == rota._fkIdMotorista)
                        .Select(u => u._nome)
                        .FirstOrDefault(),
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
                var rota = _context.Rotas.Find(id);
                if (rota == null)
                    return NotFound(new { message = "Rota não encontrada" });

                _context.Rotas.Remove(rota);
                _context.SaveChanges();
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
