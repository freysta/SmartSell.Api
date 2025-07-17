using Microsoft.AspNetCore.Mvc;
using SmartSell.Api.DAO;
using SmartSell.Api.Data;
using SmartSell.Api.Models.Galdino;

namespace SmartSell.Api.Controllers.Galdino
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly UsuarioDAO _usuarioDAO;

        public AdminController(GaldinoDbContext context)
        {
            _usuarioDAO = new UsuarioDAO(context);
        }

        [HttpPost("create-admin")]
        public IActionResult CreateAdmin([FromBody] CreateAdminRequest request)
        {
            try
            {
                var existingUser = _usuarioDAO.GetByEmail(request.Email);

                if (existingUser != null)
                {
                    return BadRequest("Email já está em uso");
                }

                var admin = new Usuario
                {
                    _nome = request.Name,
                    _email = request.Email,
                    _senha = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    _ativo = true
                };

                _usuarioDAO.Create(admin);

                var response = new
                {
                    id = admin._id,
                    name = admin._nome,
                    email = admin._email,
                    phone = request.Phone ?? "",
                    role = "Admin",
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
                var usuarios = _usuarioDAO.GetAll();
                var admins = usuarios
                    .Where(u => u._ativo == true)
                    .Select(a => new
                    {
                        id = a._id,
                        name = a._nome,
                        email = a._email,
                        phone = "",
                        role = "Admin",
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
                // Remover verificação de primeiro admin

                var existingUser = _usuarioDAO.GetByEmail(request.Email);

                if (existingUser != null)
                {
                    return BadRequest("Email já está em uso");
                }

                var admin = new Usuario
                {
                    _nome = request.Name,
                    _email = request.Email,
                    _senha = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    _ativo = true
                };

                _usuarioDAO.Create(admin);

                var response = new
                {
                    id = admin._id,
                    name = admin._nome,
                    email = admin._email,
                    phone = request.Phone ?? "",
                    role = "Admin",
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
