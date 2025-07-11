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
        public IActionResult GetAll()
        {
            try
            {
                var rotas = _context.Rotas
                    .Select(r => new
                    {
                        id = r._id,
                        name = r._destino,
                        origin = "Terminal Rodoviário", // Valor padrão
                        destination = r._destino,
                        departureTime = r._horarioSaida.ToString(@"hh\:mm"),
                        arrivalTime = r._horarioSaida.Add(TimeSpan.FromHours(1)).ToString(@"hh\:mm"), // +1 hora
                        driverId = r._fkIdMotorista,
                        status = r._status,
                        capacity = 40, // Valor padrão
                        currentPassengers = 28, // Valor padrão
                        createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
                    }).ToList();

                return Ok(rotas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var rota = _context.Rotas.Find(id);
                if (rota == null)
                    return NotFound("Rota não encontrada");

                var response = new
                {
                    id = rota._id,
                    name = rota._destino,
                    origin = "Terminal Rodoviário",
                    destination = rota._destino,
                    departureTime = rota._horarioSaida.ToString(@"hh\:mm"),
                    arrivalTime = rota._horarioSaida.Add(TimeSpan.FromHours(1)).ToString(@"hh\:mm"),
                    driverId = rota._fkIdMotorista,
                    status = rota._status,
                    capacity = 40,
                    currentPassengers = 28,
                    createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateRouteRequest request)
        {
            try
            {
                // Verificar se motorista existe
                var motorista = _context.Usuarios
                    .FirstOrDefault(u => u._id == request.DriverId && u._tipo == "Motorista");

                if (motorista == null)
                {
                    return BadRequest("Motorista não encontrado");
                }

                var rota = new Rota
                {
                    _dataRota = request.DepartureDate ?? DateTime.Now,
                    _destino = request.Destination,
                    _horarioSaida = request.DepartureTime,
                    _status = "Ativa",
                    _fkIdMotorista = request.DriverId
                };

                _context.Rotas.Add(rota);
                _context.SaveChanges();

                var response = new
                {
                    id = rota._id,
                    name = rota._destino,
                    origin = request.Origin ?? "Terminal Rodoviário",
                    destination = rota._destino,
                    departureTime = rota._horarioSaida.ToString(@"hh\:mm"),
                    arrivalTime = rota._horarioSaida.Add(TimeSpan.FromHours(1)).ToString(@"hh\:mm"),
                    driverId = rota._fkIdMotorista,
                    status = rota._status,
                    capacity = 40,
                    currentPassengers = 0,
                    createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateRouteRequest request)
        {
            try
            {
                var rota = _context.Rotas.Find(id);
                if (rota == null)
                    return NotFound("Rota não encontrada");

                // Verificar se novo motorista existe (se fornecido)
                if (request.DriverId.HasValue)
                {
                    var motorista = _context.Usuarios
                        .FirstOrDefault(u => u._id == request.DriverId && u._tipo == "Motorista");

                    if (motorista == null)
                    {
                        return BadRequest("Motorista não encontrado");
                    }
                }

                rota._destino = request.Destination ?? rota._destino;
                rota._horarioSaida = request.DepartureTime ?? rota._horarioSaida;
                rota._fkIdMotorista = request.DriverId ?? rota._fkIdMotorista;
                rota._dataRota = request.DepartureDate ?? rota._dataRota;

                if (!string.IsNullOrEmpty(request.Status))
                {
                    rota._status = request.Status;
                }

                _context.SaveChanges();

                var response = new
                {
                    id = rota._id,
                    name = rota._destino,
                    origin = "Terminal Rodoviário",
                    destination = rota._destino,
                    departureTime = rota._horarioSaida.ToString(@"hh\:mm"),
                    arrivalTime = rota._horarioSaida.Add(TimeSpan.FromHours(1)).ToString(@"hh\:mm"),
                    driverId = rota._fkIdMotorista,
                    status = rota._status,
                    capacity = 40,
                    currentPassengers = 28,
                    createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var rota = _context.Rotas.Find(id);
                if (rota == null)
                    return NotFound("Rota não encontrada");

                _context.Rotas.Remove(rota);
                _context.SaveChanges();
                return Ok("Rota removida com sucesso");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }

    public class CreateRouteRequest
    {
        public string Destination { get; set; } = string.Empty;
        public string? Origin { get; set; }
        public TimeSpan DepartureTime { get; set; }
        public DateTime? DepartureDate { get; set; }
        public int DriverId { get; set; }
    }

    public class UpdateRouteRequest
    {
        public string? Destination { get; set; }
        public string? Origin { get; set; }
        public TimeSpan? DepartureTime { get; set; }
        public DateTime? DepartureDate { get; set; }
        public int? DriverId { get; set; }
        public string? Status { get; set; }
    }
}
