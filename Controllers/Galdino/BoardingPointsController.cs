using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartSell.Api.Data;
using SmartSell.Api.Models.Galdino;
using System.Text.Json;

namespace SmartSell.Api.Controllers.Galdino
{
    [ApiController]
    [Route("api/boarding-points")]
    public class BoardingPointsController : ControllerBase
    {
        private readonly GaldinoDbContext _context;

        public BoardingPointsController(GaldinoDbContext context)
        {
            _context = context;
        }

        // GET: api/boarding-points
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetBoardingPoints()
        {
            var pontosEmbarque = await _context.PontosEmbarque.ToListAsync();

            var result = pontosEmbarque.Select(p => new
            {
                id = p._id,
                name = p._name,
                address = p._address,
                neighborhood = p._neighborhood,
                city = p._city,
                coordinates = p._lat.HasValue && p._lng.HasValue ? new
                {
                    lat = p._lat.Value,
                    lng = p._lng.Value
                } : null,
                status = p._status,
                routes = ParseJsonArray(p._routes),
                createdAt = p._createdAt.ToString("yyyy-MM-ddTHH:mm:ssZ")
            });

            return Ok(result);
        }

        // GET: api/boarding-points/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetBoardingPoint(int id)
        {
            var pontoEmbarque = await _context.PontosEmbarque.FindAsync(id);

            if (pontoEmbarque == null)
            {
                return NotFound(new { error = new { message = "Ponto de embarque não encontrado", code = "BOARDING_POINT_NOT_FOUND" } });
            }

            var result = new
            {
                id = pontoEmbarque._id,
                name = pontoEmbarque._name,
                address = pontoEmbarque._address,
                neighborhood = pontoEmbarque._neighborhood,
                city = pontoEmbarque._city,
                coordinates = pontoEmbarque._lat.HasValue && pontoEmbarque._lng.HasValue ? new
                {
                    lat = pontoEmbarque._lat.Value,
                    lng = pontoEmbarque._lng.Value
                } : null,
                status = pontoEmbarque._status,
                routes = ParseJsonArray(pontoEmbarque._routes),
                createdAt = pontoEmbarque._createdAt.ToString("yyyy-MM-ddTHH:mm:ssZ")
            };

            return Ok(result);
        }

        // POST: api/boarding-points
        [HttpPost]
        public async Task<ActionResult<object>> CreateBoardingPoint([FromBody] JsonElement body)
        {
            try
            {
                var pontoEmbarque = new PontoEmbarque
                {
                    _name = body.GetProperty("name").GetString() ?? "",
                    _address = body.GetProperty("address").GetString() ?? "",
                    _neighborhood = body.GetProperty("neighborhood").GetString() ?? "",
                    _city = body.GetProperty("city").GetString() ?? "",
                    _status = body.GetProperty("status").GetString() ?? "active",
                    _createdAt = DateTime.Now,
                    _routes = "[]"
                };

                if (body.TryGetProperty("coordinates", out var coordinatesElement))
                {
                    if (coordinatesElement.TryGetProperty("lat", out var latElement))
                        pontoEmbarque._lat = latElement.GetDouble();
                    
                    if (coordinatesElement.TryGetProperty("lng", out var lngElement))
                        pontoEmbarque._lng = lngElement.GetDouble();
                }

                if (body.TryGetProperty("routes", out var routesElement))
                {
                    var routes = routesElement.EnumerateArray().Select(x => x.GetInt32()).ToArray();
                    pontoEmbarque._routes = JsonSerializer.Serialize(routes);
                }

                _context.PontosEmbarque.Add(pontoEmbarque);
                await _context.SaveChangesAsync();

                var result = new
                {
                    id = pontoEmbarque._id,
                    name = pontoEmbarque._name,
                    address = pontoEmbarque._address,
                    neighborhood = pontoEmbarque._neighborhood,
                    city = pontoEmbarque._city,
                    coordinates = pontoEmbarque._lat.HasValue && pontoEmbarque._lng.HasValue ? new
                    {
                        lat = pontoEmbarque._lat.Value,
                        lng = pontoEmbarque._lng.Value
                    } : null,
                    status = pontoEmbarque._status,
                    routes = ParseJsonArray(pontoEmbarque._routes),
                    createdAt = pontoEmbarque._createdAt.ToString("yyyy-MM-ddTHH:mm:ssZ")
                };

                return CreatedAtAction(nameof(GetBoardingPoint), new { id = pontoEmbarque._id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = new { message = "Dados inválidos", code = "INVALID_DATA", details = ex.Message } });
            }
        }

        // PUT: api/boarding-points/5
        [HttpPut("{id}")]
        public async Task<ActionResult<object>> UpdateBoardingPoint(int id, [FromBody] JsonElement body)
        {
            var pontoEmbarque = await _context.PontosEmbarque.FindAsync(id);
            if (pontoEmbarque == null)
            {
                return NotFound(new { error = new { message = "Ponto de embarque não encontrado", code = "BOARDING_POINT_NOT_FOUND" } });
            }

            try
            {
                if (body.TryGetProperty("name", out var nameElement))
                    pontoEmbarque._name = nameElement.GetString() ?? pontoEmbarque._name;

                if (body.TryGetProperty("address", out var addressElement))
                    pontoEmbarque._address = addressElement.GetString() ?? pontoEmbarque._address;

                if (body.TryGetProperty("neighborhood", out var neighborhoodElement))
                    pontoEmbarque._neighborhood = neighborhoodElement.GetString() ?? pontoEmbarque._neighborhood;

                if (body.TryGetProperty("city", out var cityElement))
                    pontoEmbarque._city = cityElement.GetString() ?? pontoEmbarque._city;

                if (body.TryGetProperty("status", out var statusElement))
                    pontoEmbarque._status = statusElement.GetString() ?? pontoEmbarque._status;

                if (body.TryGetProperty("coordinates", out var coordinatesElement))
                {
                    if (coordinatesElement.TryGetProperty("lat", out var latElement))
                        pontoEmbarque._lat = latElement.GetDouble();
                    
                    if (coordinatesElement.TryGetProperty("lng", out var lngElement))
                        pontoEmbarque._lng = lngElement.GetDouble();
                }

                if (body.TryGetProperty("routes", out var routesElement))
                {
                    var routes = routesElement.EnumerateArray().Select(x => x.GetInt32()).ToArray();
                    pontoEmbarque._routes = JsonSerializer.Serialize(routes);
                }

                await _context.SaveChangesAsync();

                var result = new
                {
                    id = pontoEmbarque._id,
                    name = pontoEmbarque._name,
                    address = pontoEmbarque._address,
                    neighborhood = pontoEmbarque._neighborhood,
                    city = pontoEmbarque._city,
                    coordinates = pontoEmbarque._lat.HasValue && pontoEmbarque._lng.HasValue ? new
                    {
                        lat = pontoEmbarque._lat.Value,
                        lng = pontoEmbarque._lng.Value
                    } : null,
                    status = pontoEmbarque._status,
                    routes = ParseJsonArray(pontoEmbarque._routes),
                    createdAt = pontoEmbarque._createdAt.ToString("yyyy-MM-ddTHH:mm:ssZ")
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = new { message = "Dados inválidos", code = "INVALID_DATA", details = ex.Message } });
            }
        }

        // DELETE: api/boarding-points/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBoardingPoint(int id)
        {
            var pontoEmbarque = await _context.PontosEmbarque.FindAsync(id);
            if (pontoEmbarque == null)
            {
                return NotFound(new { error = new { message = "Ponto de embarque não encontrado", code = "BOARDING_POINT_NOT_FOUND" } });
            }

            _context.PontosEmbarque.Remove(pontoEmbarque);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private int[]? ParseJsonArray(string? jsonString)
        {
            if (string.IsNullOrEmpty(jsonString))
                return new int[0];

            try
            {
                return JsonSerializer.Deserialize<int[]>(jsonString);
            }
            catch
            {
                return new int[0];
            }
        }
    }
}
