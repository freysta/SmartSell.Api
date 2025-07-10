using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartSell.Api.Data;
using SmartSell.Api.DTOs;
using SmartSell.Api.Models.Galdino;

namespace SmartSell.Api.Controllers.Galdino
{
    [ApiController]
    [Route("api/routes")]
    [Authorize]
    public class RoutesController : ControllerBase
    {
        private readonly GaldinoDbContext _context;

        public RoutesController(GaldinoDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RouteDto>>> GetRoutes()
        {
            try
            {
                var routes = await _context.Rotas
                    .Include(r => r.Motorista)
                    .Include(r => r.RotaAlunos)
                    .ToListAsync();

                var routeDtos = routes.Select(r => new RouteDto
                {
                    Id = r.IdRota,
                    Name = $"Rota para {r.Destino}", // Nome gerado baseado no destino
                    Origin = "Terminal Central", // Valor padrão - seria necessário adicionar campo no modelo
                    Destination = r.Destino,
                    DepartureTime = r.HorarioSaida.ToString(@"hh\:mm"),
                    ArrivalTime = r.HorarioSaida.Add(TimeSpan.FromHours(1)).ToString(@"hh\:mm"), // Estimativa
                    DriverId = r.FkIdMotorista,
                    Status = r.Status.ToString().ToLower(),
                    Capacity = 40, // Valor padrão - seria necessário adicionar campo no modelo
                    CurrentPassengers = r.RotaAlunos.Count,
                    CreatedAt = r.DataRota
                }).ToList();

                return Ok(routeDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RouteDto>> GetRoute(int id)
        {
            try
            {
                var route = await _context.Rotas
                    .Include(r => r.Motorista)
                    .Include(r => r.RotaAlunos)
                    .FirstOrDefaultAsync(r => r.IdRota == id);

                if (route == null)
                {
                    return NotFound(new { message = "Rota não encontrada" });
                }

                var routeDto = new RouteDto
                {
                    Id = route.IdRota,
                    Name = $"Rota para {route.Destino}",
                    Origin = "Terminal Central",
                    Destination = route.Destino,
                    DepartureTime = route.HorarioSaida.ToString(@"hh\:mm"),
                    ArrivalTime = route.HorarioSaida.Add(TimeSpan.FromHours(1)).ToString(@"hh\:mm"),
                    DriverId = route.FkIdMotorista,
                    Status = route.Status.ToString().ToLower(),
                    Capacity = 40,
                    CurrentPassengers = route.RotaAlunos.Count,
                    CreatedAt = route.DataRota
                };

                return Ok(routeDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<RouteDto>> CreateRoute([FromBody] CreateRouteDto createDto)
        {
            try
            {
                // Verificar se o motorista existe
                var driver = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.IdUsuario == createDto.DriverId && u.Tipo == TipoUsuario.Motorista);

                if (driver == null)
                {
                    return BadRequest(new { message = "Motorista não encontrado" });
                }

                // Validar horários
                if (createDto.ArrivalTime <= createDto.DepartureTime)
                {
                    return BadRequest(new { message = "Horário de chegada deve ser posterior ao horário de saída" });
                }

                // Verificar se o motorista já tem rota no mesmo horário
                var conflictingRoute = await _context.Rotas
                    .Where(r => r.FkIdMotorista == createDto.DriverId &&
                               r.DataRota.Date == createDto.RouteDate.Date &&
                               r.HorarioSaida == createDto.DepartureTime &&
                               r.Status != StatusRota.Cancelada)
                    .FirstOrDefaultAsync();

                if (conflictingRoute != null)
                {
                    return BadRequest(new { message = "Motorista já possui rota agendada para este horário" });
                }

                var route = new Rota
                {
                    DataRota = createDto.RouteDate,
                    Destino = createDto.Destination,
                    HorarioSaida = createDto.DepartureTime,
                    Status = StatusRota.Planejada,
                    FkIdMotorista = createDto.DriverId
                };

                _context.Rotas.Add(route);
                await _context.SaveChangesAsync();

                var routeDto = new RouteDto
                {
                    Id = route.IdRota,
                    Name = createDto.Name,
                    Origin = createDto.Origin,
                    Destination = route.Destino,
                    DepartureTime = route.HorarioSaida.ToString(@"hh\:mm"),
                    ArrivalTime = createDto.ArrivalTime.ToString(@"hh\:mm"),
                    DriverId = route.FkIdMotorista,
                    Status = route.Status.ToString().ToLower(),
                    Capacity = createDto.Capacity,
                    CurrentPassengers = 0,
                    CreatedAt = route.DataRota
                };

                return CreatedAtAction(nameof(GetRoute), new { id = route.IdRota }, routeDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateRoute(int id, [FromBody] UpdateRouteDto updateDto)
        {
            try
            {
                var route = await _context.Rotas.FindAsync(id);

                if (route == null)
                {
                    return NotFound(new { message = "Rota não encontrada" });
                }

                // Verificar se a rota pode ser editada
                if (route.Status == StatusRota.EmAndamento || route.Status == StatusRota.Concluída)
                {
                    return BadRequest(new { message = "Não é possível editar rota em andamento ou concluída" });
                }

                // Verificar se o motorista existe (se fornecido)
                if (updateDto.DriverId.HasValue)
                {
                    var driver = await _context.Usuarios
                        .FirstOrDefaultAsync(u => u.IdUsuario == updateDto.DriverId && u.Tipo == TipoUsuario.Motorista);

                    if (driver == null)
                    {
                        return BadRequest(new { message = "Motorista não encontrado" });
                    }
                }

                // Validar horários (se fornecidos)
                var departureTime = updateDto.DepartureTime ?? route.HorarioSaida;
                var arrivalTime = updateDto.ArrivalTime ?? route.HorarioSaida.Add(TimeSpan.FromHours(1));

                if (arrivalTime <= departureTime)
                {
                    return BadRequest(new { message = "Horário de chegada deve ser posterior ao horário de saída" });
                }

                // Atualizar campos
                if (!string.IsNullOrEmpty(updateDto.Destination))
                    route.Destino = updateDto.Destination;

                if (updateDto.DepartureTime.HasValue)
                    route.HorarioSaida = updateDto.DepartureTime.Value;

                if (updateDto.DriverId.HasValue)
                    route.FkIdMotorista = updateDto.DriverId.Value;

                if (updateDto.RouteDate.HasValue)
                    route.DataRota = updateDto.RouteDate.Value;

                if (!string.IsNullOrEmpty(updateDto.Status))
                {
                    if (Enum.TryParse<StatusRota>(updateDto.Status, true, out var status))
                    {
                        route.Status = status;
                    }
                }

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteRoute(int id)
        {
            try
            {
                var route = await _context.Rotas
                    .Include(r => r.RotaAlunos)
                    .Include(r => r.Presencas)
                    .FirstOrDefaultAsync(r => r.IdRota == id);

                if (route == null)
                {
                    return NotFound(new { message = "Rota não encontrada" });
                }

                // Verificar se a rota pode ser excluída
                if (route.Status == StatusRota.EmAndamento)
                {
                    return BadRequest(new { message = "Não é possível excluir rota em andamento" });
                }

                // Se há alunos ou presenças associadas, apenas cancelar a rota
                if (route.RotaAlunos.Any() || route.Presencas.Any())
                {
                    route.Status = StatusRota.Cancelada;
                    await _context.SaveChangesAsync();
                    return Ok(new { message = "Rota cancelada devido a registros associados" });
                }

                // Se não há registros associados, pode excluir
                _context.Rotas.Remove(route);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        [HttpPatch("{id}/start")]
        [Authorize(Roles = "admin,motorista")]
        public async Task<IActionResult> StartRoute(int id)
        {
            try
            {
                var route = await _context.Rotas.FindAsync(id);

                if (route == null)
                {
                    return NotFound(new { message = "Rota não encontrada" });
                }

                if (route.Status != StatusRota.Planejada)
                {
                    return BadRequest(new { message = "Apenas rotas planejadas podem ser iniciadas" });
                }

                route.Status = StatusRota.EmAndamento;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Rota iniciada com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        [HttpPatch("{id}/complete")]
        [Authorize(Roles = "admin,motorista")]
        public async Task<IActionResult> CompleteRoute(int id)
        {
            try
            {
                var route = await _context.Rotas.FindAsync(id);

                if (route == null)
                {
                    return NotFound(new { message = "Rota não encontrada" });
                }

                if (route.Status != StatusRota.EmAndamento)
                {
                    return BadRequest(new { message = "Apenas rotas em andamento podem ser concluídas" });
                }

                route.Status = StatusRota.Concluída;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Rota concluída com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }
    }
}
