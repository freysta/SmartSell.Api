using Microsoft.AspNetCore.Mvc;
using SmartSell.Api.Data;
using SmartSell.Api.Models.Galdino;

namespace SmartSell.Api.Controllers.Galdino
{
    [ApiController]
    [Route("motorista")]
    public class MotoristaController : ControllerBase
    {
        private readonly GaldinoDbContext _context;

        public MotoristaController(GaldinoDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var motoristas = _context.Usuarios
                    .Where(u => u._tipo == "Motorista")
                    .ToList();
                return Ok(motoristas);
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
                var motorista = _context.Usuarios
                    .Where(u => u._tipo == "Motorista" && u._id == id)
                    .FirstOrDefault();
                
                if (motorista == null)
                    return NotFound("Motorista não encontrado");
                
                return Ok(motorista);
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
        public IActionResult Create([FromBody] Usuario motorista)
        {
            try
            {
                motorista._tipo = "Motorista";
                motorista._senha = BCrypt.Net.BCrypt.HashPassword(motorista._senha);
                
                _context.Usuarios.Add(motorista);
                _context.SaveChanges();
                return Ok(motorista);
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
        public IActionResult Update(int id, [FromBody] Usuario motorista)
        {
            try
            {
                var motoristaExistente = _context.Usuarios
                    .Where(u => u._tipo == "Motorista" && u._id == id)
                    .FirstOrDefault();
                
                if (motoristaExistente == null)
                    return NotFound("Motorista não encontrado");

                motoristaExistente._nome = motorista._nome;
                motoristaExistente._email = motorista._email;
                motoristaExistente._telefone = motorista._telefone;
                
                if (!string.IsNullOrEmpty(motorista._senha))
                {
                    motoristaExistente._senha = BCrypt.Net.BCrypt.HashPassword(motorista._senha);
                }

                _context.SaveChanges();
                return Ok(motoristaExistente);
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
                var motorista = _context.Usuarios
                    .Where(u => u._tipo == "Motorista" && u._id == id)
                    .FirstOrDefault();
                
                if (motorista == null)
                    return NotFound("Motorista não encontrado");

                _context.Usuarios.Remove(motorista);
                _context.SaveChanges();
                return Ok("Motorista removido com sucesso");
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
