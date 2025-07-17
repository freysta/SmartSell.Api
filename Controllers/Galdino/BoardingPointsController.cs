using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartSell.Api.DAO;
using SmartSell.Api.Data;
using SmartSell.Api.Models.Galdino;
using System.Text.Json;

namespace SmartSell.Api.Controllers.Galdino
{
    [ApiController]
    [Route("api/boarding-points")]
    public class BoardingPointsController : ControllerBase
    {
        private readonly PontoEmbarqueDAO _pontoEmbarqueDAO;

        public BoardingPointsController(GaldinoDbContext context)
        {
            _pontoEmbarqueDAO = new PontoEmbarqueDAO(context);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetBoardingPoints()
        {
            var pontosEmbarque = _pontoEmbarqueDAO.GetAll();

            var result = pontosEmbarque.Select(p => new
            {
                id = p._id,
                name = p._nome,
                address = p._rua,
                neighborhood = p._bairro,
                city = p._cidade,
                coordinates = p._latitude.HasValue && p._longitude.HasValue ? new
                {
                    lat = p._latitude.Value,
                    lng = p._longitude.Value
                } : null,
                status = "active",
                routes = 0,
                createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetBoardingPoint(int id)
        {
            var pontoEmbarque = _pontoEmbarqueDAO.GetById(id);

            if (pontoEmbarque == null)
            {
                return NotFound(new { error = new { message = "Ponto de embarque não encontrado", code = "BOARDING_POINT_NOT_FOUND" } });
            }

            var result = new
            {
                id = pontoEmbarque._id,
                name = pontoEmbarque._nome,
                address = pontoEmbarque._rua,
                neighborhood = pontoEmbarque._bairro,
                city = pontoEmbarque._cidade,
                coordinates = pontoEmbarque._latitude.HasValue && pontoEmbarque._longitude.HasValue ? new
                {
                    lat = pontoEmbarque._latitude.Value,
                    lng = pontoEmbarque._longitude.Value
                } : null,
                status = "active",
                routes = 0,
                createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
            };

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<object>> CreateBoardingPoint([FromBody] JsonElement body)
        {
            try
            {
                var pontoEmbarque = new PontoEmbarque
                {
                    _nome = body.GetProperty("name").GetString() ?? "",
                    _rua = body.GetProperty("address").GetString() ?? "",
                    _bairro = body.GetProperty("neighborhood").GetString() ?? "",
                    _cidade = body.GetProperty("city").GetString() ?? ""
                };

                if (body.TryGetProperty("coordinates", out var coordinatesElement))
                {
                    if (coordinatesElement.TryGetProperty("lat", out var latElement))
                        pontoEmbarque._latitude = (decimal)latElement.GetDouble();
                    
                    if (coordinatesElement.TryGetProperty("lng", out var lngElement))
                        pontoEmbarque._longitude = (decimal)lngElement.GetDouble();
                }

                _pontoEmbarqueDAO.Create(pontoEmbarque);

                var result = new
                {
                    id = pontoEmbarque._id,
                    name = pontoEmbarque._nome,
                    address = pontoEmbarque._rua,
                    neighborhood = pontoEmbarque._bairro,
                    city = pontoEmbarque._cidade,
                    coordinates = pontoEmbarque._latitude.HasValue && pontoEmbarque._longitude.HasValue ? new
                    {
                        lat = pontoEmbarque._latitude.Value,
                        lng = pontoEmbarque._longitude.Value
                    } : null,
                    status = "active",
                    routes = 0,
                    createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
                };

                return CreatedAtAction(nameof(GetBoardingPoint), new { id = pontoEmbarque._id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = new { message = "Dados inválidos", code = "INVALID_DATA", details = ex.Message } });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<object>> UpdateBoardingPoint(int id, [FromBody] JsonElement body)
        {
            var pontoEmbarque = _pontoEmbarqueDAO.GetById(id);
            if (pontoEmbarque == null)
            {
                return NotFound(new { error = new { message = "Ponto de embarque não encontrado", code = "BOARDING_POINT_NOT_FOUND" } });
            }

            try
            {
                if (body.TryGetProperty("name", out var nameElement))
                    pontoEmbarque._nome = nameElement.GetString() ?? pontoEmbarque._nome;

                if (body.TryGetProperty("address", out var addressElement))
                    pontoEmbarque._rua = addressElement.GetString() ?? pontoEmbarque._rua;

                if (body.TryGetProperty("neighborhood", out var neighborhoodElement))
                    pontoEmbarque._bairro = neighborhoodElement.GetString() ?? pontoEmbarque._bairro;

                if (body.TryGetProperty("city", out var cityElement))
                    pontoEmbarque._cidade = cityElement.GetString() ?? pontoEmbarque._cidade;

                if (body.TryGetProperty("coordinates", out var coordinatesElement))
                {
                    if (coordinatesElement.TryGetProperty("lat", out var latElement))
                        pontoEmbarque._latitude = (decimal)latElement.GetDouble();
                    
                    if (coordinatesElement.TryGetProperty("lng", out var lngElement))
                        pontoEmbarque._longitude = (decimal)lngElement.GetDouble();
                }

                _pontoEmbarqueDAO.Update(pontoEmbarque);

                var result = new
                {
                    id = pontoEmbarque._id,
                    name = pontoEmbarque._nome,
                    address = pontoEmbarque._rua,
                    neighborhood = pontoEmbarque._bairro,
                    city = pontoEmbarque._cidade,
                    coordinates = pontoEmbarque._latitude.HasValue && pontoEmbarque._longitude.HasValue ? new
                    {
                        lat = pontoEmbarque._latitude.Value,
                        lng = pontoEmbarque._longitude.Value
                    } : null,
                    status = "active",
                    routes = 0,
                    createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = new { message = "Dados inválidos", code = "INVALID_DATA", details = ex.Message } });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBoardingPoint(int id)
        {
            var pontoEmbarque = _pontoEmbarqueDAO.GetById(id);
            if (pontoEmbarque == null)
            {
                return NotFound(new { error = new { message = "Ponto de embarque não encontrado", code = "BOARDING_POINT_NOT_FOUND" } });
            }

            _pontoEmbarqueDAO.Delete(id);

            return NoContent();
        }
    }
}
