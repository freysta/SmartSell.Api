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
                    .Where(u => u.Tipo == TipoUsuario.Motorista)
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
                    .Where(u => u.Tipo == TipoUsuario.Motorista && u.IdUsuario == id)
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
                motorista.Tipo = TipoUsuario.Motorista;
                motorista.Senha = BCrypt.Net.BCrypt.HashPassword(motorista.Senha);
                
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
                    .Where(u => u.Tipo == TipoUsuario.Motorista && u.IdUsuario == id)
                    .FirstOrDefault();
                
                if (motoristaExistente == null)
                    return NotFound("Motorista não encontrado");

                motoristaExistente.Nome = motorista.Nome;
                motoristaExistente.Email = motorista.Email;
                motoristaExistente.Telefone = motorista.Telefone;
                
                if (!string.IsNullOrEmpty(motorista.Senha))
                {
                    motoristaExistente.Senha = BCrypt.Net.BCrypt.HashPassword(motorista.Senha);
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
                    .Where(u => u.Tipo == TipoUsuario.Motorista && u.IdUsuario == id)
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
