using Microsoft.AspNetCore.Mvc;
using SmartSell.Api.DAO;
using SmartSell.Api.Data;
using SmartSell.Api.Models.Galdino;

namespace SmartSell.Api.Controllers.Galdino
{
    [ApiController]
    [Route("api/drivers")]
    public class DriversController : ControllerBase
    {
        private readonly UsuarioDAO _usuarioDAO;
        private readonly RotaDAO _rotaDAO;

        public DriversController(GaldinoDbContext context)
        {
            _usuarioDAO = new UsuarioDAO(context);
            _rotaDAO = new RotaDAO(context);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var motoristas = _usuarioDAO.GetAll()
                    .Select(m => new
                    {
                        id = m._id,
                        name = m._nome,
                        email = m._email,
                        phone = (string?)null,
                        cnh = (string?)null,
                        vehicle = (string?)null,
                        licenseExpiry = (string?)null,
                        status = "active",
                        createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
                    }).ToList();

                return Ok(new { data = motoristas, message = "Motoristas listados com sucesso" });
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

                var response = new
                {
                    id = motorista._id,
                    name = motorista._nome,
                    email = motorista._email,
                    phone = (string?)null,
                    cnh = (string?)null,
                    vehicle = (string?)null,
                    licenseExpiry = (string?)null,
                    status = "active",
                    createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
                };

                return Ok(new { data = response, message = "Motorista encontrado" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateDriverRequest request)
        {
            try
            {
                var existingUser = _usuarioDAO.GetByEmail(request.Email);

                if (existingUser != null)
                {
                    return BadRequest(new { message = "Email já está em uso" });
                }

                var motorista = new Usuario
                {
                    _nome = request.Name,
                    _email = request.Email,
                    _senha = BCrypt.Net.BCrypt.HashPassword(request.Password ?? "TempPass123!")
                };

                _usuarioDAO.Create(motorista);

                var response = new
                {
                    id = motorista._id,
                    name = motorista._nome,
                    email = motorista._email,
                    phone = (string?)null,
                    cnh = request.Cnh,
                    vehicle = request.Vehicle,
                    licenseExpiry = request.LicenseExpiry,
                    status = "active",
                    createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
                };

                return StatusCode(201, new { data = response, message = "Motorista criado com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateDriverRequest request)
        {
            try
            {
                var motorista = _usuarioDAO.GetById(id);

                if (motorista == null)
                    return NotFound("Motorista não encontrado");

                if (!string.IsNullOrEmpty(request.Email) && request.Email != motorista._email)
                {
                    var existingUser = _usuarioDAO.GetByEmail(request.Email);

                    if (existingUser != null && existingUser._id != id)
                    {
                        return BadRequest(new { message = "Email já está em uso" });
                    }
                }

                motorista._nome = request.Name ?? motorista._nome;
                motorista._email = request.Email ?? motorista._email;

                _usuarioDAO.Update(motorista);

                var response = new
                {
                    id = motorista._id,
                    name = motorista._nome,
                    email = motorista._email,
                    phone = (string?)null,
                    cnh = request.Cnh,
                    vehicle = request.Vehicle,
                    licenseExpiry = request.LicenseExpiry,
                    status = "active",
                    createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
                };

                return Ok(new { data = response, message = "Motorista atualizado com sucesso" });
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

                var rotasAssociadas = _rotaDAO.GetAll()
                    .Any(r => r._motoristaId == id);

                if (rotasAssociadas)
                {
                    return BadRequest(new { message = "Não é possível excluir motorista com rotas associadas" });
                }

                _usuarioDAO.Delete(id);
                return Ok(new { message = "Motorista removido com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }

    public class CreateDriverRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Password { get; set; }
        public string? Phone { get; set; }
        public string? Cnh { get; set; }
        public string? Vehicle { get; set; }
        public string? LicenseExpiry { get; set; }
    }

    public class UpdateDriverRequest
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Cnh { get; set; }
        public string? Vehicle { get; set; }
        public string? LicenseExpiry { get; set; }
    }
}
