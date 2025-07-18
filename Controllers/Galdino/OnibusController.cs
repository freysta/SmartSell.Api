using Microsoft.AspNetCore.Mvc;
using SmartSell.Api.DAO;
using SmartSell.Api.Data;
using SmartSell.Api.Models.Galdino;

namespace SmartSell.Api.Controllers.Galdino
{
    [ApiController]
    [Route("api/onibus")]
    public class OnibusController : ControllerBase
    {
        private readonly OnibusDAO _onibusDAO;

        public OnibusController(GaldinoDbContext context)
        {
            _onibusDAO = new OnibusDAO(context);
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] string? placa = null, [FromQuery] string? status = null)
        {
            try
            {
                List<Onibus> onibus;

                if (!string.IsNullOrEmpty(status) && Enum.TryParse<StatusOnibusEnum>(status, out var statusEnum))
                {
                    onibus = _onibusDAO.GetByStatus(statusEnum);
                }
                else
                {
                    onibus = _onibusDAO.GetAll(placa ?? "");
                }

                var result = onibus.Select(o => new
                {
                    id = o._id,
                    placa = o._placa,
                    modelo = o._modelo,
                    capacidade = o._capacidade,
                    ano = o._ano,
                    status = o._status.ToString(),
                    createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
                }).ToList();

                return Ok(new { data = result, message = "Ônibus listados com sucesso" });
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
                var onibus = _onibusDAO.GetById(id);

                if (onibus == null)
                    return NotFound("Ônibus não encontrado");

                var response = new
                {
                    id = onibus._id,
                    placa = onibus._placa,
                    modelo = onibus._modelo,
                    capacidade = onibus._capacidade,
                    ano = onibus._ano,
                    status = onibus._status.ToString(),
                    createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
                };

                return Ok(new { data = response, message = "Ônibus encontrado" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("placa/{placa}")]
        public IActionResult GetByPlaca(string placa)
        {
            try
            {
                var onibus = _onibusDAO.GetByPlaca(placa);

                if (onibus == null)
                    return NotFound("Ônibus não encontrado");

                var response = new
                {
                    id = onibus._id,
                    placa = onibus._placa,
                    modelo = onibus._modelo,
                    capacidade = onibus._capacidade,
                    ano = onibus._ano,
                    status = onibus._status.ToString(),
                    createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
                };

                return Ok(new { data = response, message = "Ônibus encontrado" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateOnibusRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Placa) || string.IsNullOrEmpty(request.Modelo) ||
                    request.Capacidade <= 0 || request.Ano <= 0)
                {
                    return BadRequest(new { message = "Placa, modelo, capacidade e ano são obrigatórios" });
                }

                var existingOnibus = _onibusDAO.GetByPlaca(request.Placa);
                if (existingOnibus != null)
                {
                    return BadRequest(new { message = "Já existe um ônibus com essa placa" });
                }

                var onibus = new Onibus
                {
                    _placa = request.Placa,
                    _modelo = request.Modelo,
                    _capacidade = request.Capacidade,
                    _ano = request.Ano
                };

                if (!string.IsNullOrEmpty(request.Status) && Enum.TryParse<StatusOnibusEnum>(request.Status, out var statusEnum))
                    onibus._status = statusEnum;

                _onibusDAO.Create(onibus);

                var response = new
                {
                    id = onibus._id,
                    placa = onibus._placa,
                    modelo = onibus._modelo,
                    capacidade = onibus._capacidade,
                    ano = onibus._ano,
                    status = onibus._status.ToString(),
                    createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
                };

                return StatusCode(201, new { data = response, message = "Ônibus criado com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateOnibusRequest request)
        {
            try
            {
                var onibus = _onibusDAO.GetById(id);

                if (onibus == null)
                    return NotFound("Ônibus não encontrado");

                if (!string.IsNullOrEmpty(request.Placa) && request.Placa != onibus._placa)
                {
                    var existingOnibus = _onibusDAO.GetByPlaca(request.Placa);
                    if (existingOnibus != null)
                    {
                        return BadRequest(new { message = "Já existe um ônibus com essa placa" });
                    }
                }

                onibus._placa = request.Placa ?? onibus._placa;
                onibus._modelo = request.Modelo ?? onibus._modelo;
                onibus._capacidade = request.Capacidade ?? onibus._capacidade;
                onibus._ano = request.Ano ?? onibus._ano;

                if (!string.IsNullOrEmpty(request.Status) && Enum.TryParse<StatusOnibusEnum>(request.Status, out var statusEnum))
                    onibus._status = statusEnum;

                _onibusDAO.Update(onibus);

                var response = new
                {
                    id = onibus._id,
                    placa = onibus._placa,
                    modelo = onibus._modelo,
                    capacidade = onibus._capacidade,
                    ano = onibus._ano,
                    status = onibus._status.ToString(),
                    createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
                };

                return Ok(new { data = response, message = "Ônibus atualizado com sucesso" });
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
                var onibus = _onibusDAO.GetById(id);

                if (onibus == null)
                    return NotFound("Ônibus não encontrado");

                _onibusDAO.Delete(id);
                return Ok(new { message = "Ônibus removido com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPatch("{id}/status")]
        public IActionResult UpdateStatus(int id, [FromBody] UpdateOnibusStatusRequest request)
        {
            try
            {
                var onibus = _onibusDAO.GetById(id);

                if (onibus == null)
                    return NotFound("Ônibus não encontrado");

                if (string.IsNullOrEmpty(request.Status))
                {
                    return BadRequest(new { message = "Status é obrigatório" });
                }

                if (!Enum.TryParse<StatusOnibusEnum>(request.Status, out var statusEnum))
                {
                    return BadRequest(new { message = "Status inválido" });
                }

                onibus._status = statusEnum;
                _onibusDAO.Update(onibus);

                var response = new
                {
                    id = onibus._id,
                    placa = onibus._placa,
                    modelo = onibus._modelo,
                    capacidade = onibus._capacidade,
                    ano = onibus._ano,
                    status = onibus._status.ToString(),
                    createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
                };

                return Ok(new { data = response, message = "Status do ônibus atualizado com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }

    public class CreateOnibusRequest
    {
        public string Placa { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public int Capacidade { get; set; }
        public int Ano { get; set; }
        public string? Status { get; set; }
    }

    public class UpdateOnibusRequest
    {
        public string? Placa { get; set; }
        public string? Modelo { get; set; }
        public int? Capacidade { get; set; }
        public int? Ano { get; set; }
        public string? Status { get; set; }
    }

    public class UpdateOnibusStatusRequest
    {
        public string Status { get; set; } = string.Empty;
    }
}
