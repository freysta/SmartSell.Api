using Microsoft.AspNetCore.Mvc;
using SmartSell.Api.DAO;
using SmartSell.Api.Data;
using SmartSell.Api.Models.Galdino;

namespace SmartSell.Api.Controllers.Galdino
{
    [ApiController]
    [Route("motorista")]
    public class MotoristaController : ControllerBase
    {
        private readonly UsuarioDAO _usuarioDAO;

        public MotoristaController(GaldinoDbContext context)
        {
            _usuarioDAO = new UsuarioDAO(context);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var motoristas = _usuarioDAO.GetAll();
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
                var motorista = _usuarioDAO.GetById(id);
                
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
                motorista._senha = BCrypt.Net.BCrypt.HashPassword(motorista._senha);
                
                _usuarioDAO.Create(motorista);
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
                var motoristaExistente = _usuarioDAO.GetById(id);
                
                if (motoristaExistente == null)
                    return NotFound("Motorista não encontrado");

                motoristaExistente._nome = motorista._nome;
                motoristaExistente._email = motorista._email;
                
                if (!string.IsNullOrEmpty(motorista._senha))
                {
                    motoristaExistente._senha = BCrypt.Net.BCrypt.HashPassword(motorista._senha);
                }

                _usuarioDAO.Update(motoristaExistente);
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
                var motorista = _usuarioDAO.GetById(id);
                
                if (motorista == null)
                    return NotFound("Motorista não encontrado");

                _usuarioDAO.Delete(id);
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
