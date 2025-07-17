using Microsoft.AspNetCore.Mvc;
using SmartSell.Api.DAO;
using SmartSell.Api.Data;
using SmartSell.Api.Models.Galdino;

namespace SmartSell.Api.Controllers.Galdino
{
    [ApiController]
    [Route("rota")]
    public class RotaController : ControllerBase
    {
        private readonly RotaDAO _rotaDAO;

        public RotaController(GaldinoDbContext context)
        {
            _rotaDAO = new RotaDAO(context);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var rotas = _rotaDAO.GetAll();
                return Ok(rotas);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
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
                var rota = _rotaDAO.GetById(id);
                if (rota == null)
                    return NotFound("Rota não encontrada");
                
                return Ok(rota);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] Rota rota)
        {
            try
            {
                _rotaDAO.Create(rota);
                return Ok(rota);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Rota rota)
        {
            try
            {
                var rotaExistente = _rotaDAO.GetById(id);
                if (rotaExistente == null)
                    return NotFound("Rota não encontrada");

                rotaExistente._dataRota = rota._dataRota;
                rotaExistente._tipoRota = rota._tipoRota;
                rotaExistente._horarioSaida = rota._horarioSaida;
                rotaExistente._status = rota._status;
                rotaExistente._motoristaId = rota._motoristaId;

                _rotaDAO.Update(rotaExistente);
                return Ok(rotaExistente);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
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
                var rota = _rotaDAO.GetById(id);
                if (rota == null)
                    return NotFound("Rota não encontrada");

                _rotaDAO.Delete(id);
                return Ok("Rota removida com sucesso");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
