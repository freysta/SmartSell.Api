using Microsoft.AspNetCore.Mvc;
using SmartSell.Api.Data;
using SmartSell.Api.Models.Galdino;

namespace SmartSell.Api.Controllers.Galdino
{
    [ApiController]
    [Route("rota")]
    public class RotaController : ControllerBase
    {
        private readonly GaldinoDbContext _context;

        public RotaController(GaldinoDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var rotas = _context.Rotas.ToList();
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
                var rota = _context.Rotas.Find(id);
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
                _context.Rotas.Add(rota);
                _context.SaveChanges();
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
                var rotaExistente = _context.Rotas.Find(id);
                if (rotaExistente == null)
                    return NotFound("Rota não encontrada");

                rotaExistente._dataRota = rota._dataRota;
                rotaExistente._destino = rota._destino;
                rotaExistente._horarioSaida = rota._horarioSaida;
                rotaExistente._status = rota._status;
                rotaExistente._fkIdMotorista = rota._fkIdMotorista;

                _context.SaveChanges();
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
                var rota = _context.Rotas.Find(id);
                if (rota == null)
                    return NotFound("Rota não encontrada");

                _context.Rotas.Remove(rota);
                _context.SaveChanges();
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
