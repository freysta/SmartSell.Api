using Microsoft.AspNetCore.Mvc;
using SmartSell.Api.Data;
using SmartSell.Api.Models.Galdino;

namespace SmartSell.Api.Controllers.Galdino
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly GaldinoDbContext _context;

        public AuthController(GaldinoDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                var usuario = _context.Usuarios
                    .FirstOrDefault(u => u._email == request.Email);

                if (usuario == null || !BCrypt.Net.BCrypt.Verify(request.Password, usuario._senha))
                {
                    return Unauthorized("Credenciais inválidas");
                }

                var response = new
                {
                    user = new
                    {
                        id = usuario._id,
                        name = usuario._nome,
                        email = usuario._email,
                        role = usuario._tipo.ToLower(),
                        status = "active"
                    },
                    message = "Login realizado com sucesso"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok("Logout realizado com sucesso");
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                var usuario = _context.Usuarios
                    .FirstOrDefault(u => u._email == request.Email);

                if (usuario == null)
                {
                    return NotFound("Usuário não encontrado");
                }

                return Ok("Email de recuperação enviado");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class ResetPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
    }
}
