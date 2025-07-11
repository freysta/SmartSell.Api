using Microsoft.AspNetCore.Mvc;
using SmartSell.Api.Data;
using SmartSell.Api.Models.Galdino;

namespace SmartSell.Api.Controllers.Galdino
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly GaldinoDbContext _context;

        public AdminController(GaldinoDbContext context)
        {
            _context = context;
        }

        [HttpPost("create-admin")]
        public IActionResult CreateAdmin([FromBody] CreateAdminRequest request)
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

                var admin = new Usuario
                {
                    _nome = request.Name,
                    _email = request.Email,
                    _senha = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    _tipo = "Admin",
                    _telefone = request.Phone
                };

                _context.Usuarios.Add(admin);
                _context.SaveChanges();

                var response = new
                {
                    id = admin._id,
                    name = admin._nome,
                    email = admin._email,
                    phone = admin._telefone,
                    role = admin._tipo,
                    status = "active",
                    createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    message = "Administrador criado com sucesso"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("admins")]
        public IActionResult GetAllAdmins()
        {
            try
            {
                var admins = _context.Usuarios
                    .Where(u => u._tipo == "Admin")
                    .Select(a => new
                    {
                        id = a._id,
                        name = a._nome,
                        email = a._email,
                        phone = a._telefone,
                        role = a._tipo,
                        status = "active",
                        createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
                    }).ToList();

                return Ok(admins);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("create-first-admin")]
        public IActionResult CreateFirstAdmin([FromBody] CreateAdminRequest request)
        {
            try
            {
                // Verificar se já existe algum admin
                var existingAdmin = _context.Usuarios
                    .FirstOrDefault(u => u._tipo == "Admin");

                if (existingAdmin != null)
                {
                    return BadRequest("Já existe um administrador no sistema. Use o endpoint /create-admin");
                }

                // Verificar se email já existe
                var existingUser = _context.Usuarios
                    .FirstOrDefault(u => u._email == request.Email);

                if (existingUser != null)
                {
                    return BadRequest("Email já está em uso");
                }

                var admin = new Usuario
                {
                    _nome = request.Name,
                    _email = request.Email,
                    _senha = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    _tipo = "Admin",
                    _telefone = request.Phone
                };

                _context.Usuarios.Add(admin);
                _context.SaveChanges();

                var response = new
                {
                    id = admin._id,
                    name = admin._nome,
                    email = admin._email,
                    phone = admin._telefone,
                    role = admin._tipo,
                    status = "active",
                    createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    message = "Primeiro administrador criado com sucesso"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }

    public class CreateAdminRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Phone { get; set; }
    }
}
