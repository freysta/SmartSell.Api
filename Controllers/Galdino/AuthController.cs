using Microsoft.AspNetCore.Mvc;
using SmartSell.Api.DAO;
using SmartSell.Api.Data;
using SmartSell.Api.Models.Galdino;
using SmartSell.Api.Services;

namespace SmartSell.Api.Controllers.Galdino
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UsuarioDAO _usuarioDAO;
        private readonly JwtService _jwtService;

        public AuthController(GaldinoDbContext context, JwtService jwtService)
        {
            _usuarioDAO = new UsuarioDAO(context);
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                var usuario = _usuarioDAO.GetByEmail(request.Email);

                if (usuario == null || !BCrypt.Net.BCrypt.Verify(request.Password, usuario._senha))
                {
                    return Unauthorized(new { message = "Credenciais inválidas" });
                }

                var token = _jwtService.GenerateToken(usuario._id, usuario._email, "Usuario");
                var refreshToken = _jwtService.GenerateRefreshToken();

                var response = new
                {
                    user = new
                    {
                        id = usuario._id,
                        name = usuario._nome,
                        email = usuario._email,
                        phone = "",
                        role = "usuario",
                        status = "active"
                    },
                    token = token,
                    refreshToken = refreshToken,
                    message = "Login realizado com sucesso"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok(new { message = "Logout realizado com sucesso" });
        }

        [HttpPost("refresh-token")]
        public IActionResult RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.RefreshToken))
                {
                    return BadRequest(new { message = "Refresh token inválido" });
                }

                var usuario = _usuarioDAO.GetById(request.UserId);
                if (usuario == null)
                {
                    return NotFound(new { message = "Usuário não encontrado" });
                }

                var newToken = _jwtService.GenerateToken(usuario._id, usuario._email, "Usuario");
                var newRefreshToken = _jwtService.GenerateRefreshToken();

                var response = new
                {
                    token = newToken,
                    refreshToken = newRefreshToken
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                var usuario = _usuarioDAO.GetByEmail(request.Email);

                if (usuario == null)
                {
                    return NotFound(new { message = "Usuário não encontrado" });
                }

                return Ok(new { message = "Email de recuperação enviado" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
        public int UserId { get; set; }
    }

    public class ResetPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
    }
}
