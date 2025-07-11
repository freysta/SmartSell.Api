using Microsoft.AspNetCore.Mvc;
using SmartSell.Api.Data;
using SmartSell.Api.Models.Galdino;

namespace SmartSell.Api.Controllers.Galdino
{
    [ApiController]
    [Route("api/drivers")]
    public class DriversController : ControllerBase
    {
        private readonly GaldinoDbContext _context;

        public DriversController(GaldinoDbContext context)
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

                return Ok(motoristas);
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

                return Ok(response);
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
                // Verificar se email já existe
                var existingUser = _context.Usuarios
                    .FirstOrDefault(u => u._email == request.Email);

                if (existingUser != null)
                {
                    return BadRequest("Email já está em uso");
                }

                var motorista = new Usuario
                {
                    _nome = request.Name,
                    _email = request.Email,
                    _senha = BCrypt.Net.BCrypt.HashPassword(request.Password ?? "TempPass123!"), // Senha temporária se não fornecida
                    _tipo = "Motorista"
                };

                _context.Usuarios.Add(motorista);
                _context.SaveChanges();

                var response = new
                {
                    id = motorista._id,
                    name = motorista._nome,
                    email = motorista._email,
                    phone = request.Phone,
                    cnh = request.Cnh,
                    vehicle = request.Vehicle,
                    licenseExpiry = request.LicenseExpiry,
                    status = "active",
                    createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
                };

                return Ok(response);
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
                var motorista = _context.Usuarios
                    .Where(u => u._tipo == "Motorista" && u._id == id)
                    .FirstOrDefault();

                if (motorista == null)
                    return NotFound("Motorista não encontrado");

                // Verificar se novo email já existe (se fornecido)
                if (!string.IsNullOrEmpty(request.Email) && request.Email != motorista._email)
                {
                    var existingUser = _context.Usuarios
                        .FirstOrDefault(u => u._email == request.Email && u._id != id);

                    if (existingUser != null)
                    {
                        return BadRequest("Email já está em uso");
                    }
                }

                motorista._nome = request.Name ?? motorista._nome;
                motorista._email = request.Email ?? motorista._email;

                _context.SaveChanges();

                var response = new
                {
                    id = motorista._id,
                    name = motorista._nome,
                    email = motorista._email,
                    phone = request.Phone,
                    cnh = request.Cnh,
                    vehicle = request.Vehicle,
                    licenseExpiry = request.LicenseExpiry,
                    status = "active",
                    createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
                };

                return Ok(response);
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

                // Verificar se motorista tem rotas associadas
                var rotasAssociadas = _context.Rotas
                    .Where(r => r._fkIdMotorista == id)
                    .Any();

                if (rotasAssociadas)
                {
                    return BadRequest("Não é possível excluir motorista com rotas associadas");
                }

                _context.Usuarios.Remove(motorista);
                _context.SaveChanges();
                return Ok("Motorista removido com sucesso");
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
