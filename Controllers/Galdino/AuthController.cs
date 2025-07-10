using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartSell.Api.Data;
using SmartSell.Api.DTOs;
using SmartSell.Api.Services;
using BCrypt.Net;

namespace SmartSell.Api.Controllers.Galdino
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly GaldinoDbContext _context;
        private readonly JwtService _jwtService;

        public AuthController(GaldinoDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

                if (usuario == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, usuario.Senha))
                {
                    return Unauthorized(new { message = "Email ou senha incorretos" });
                }

                var token = _jwtService.GenerateToken(usuario);

                var response = new LoginResponseDto
                {
                    User = new UserDto
                    {
                        Id = usuario.IdUsuario,
                        Name = usuario.Nome,
                        Email = usuario.Email,
                        Role = usuario.Tipo.ToString().ToLower(),
                        Status = "active"
                    },
                    Token = token
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Em uma implementação real, você poderia invalidar o token
            // Por enquanto, apenas retornamos sucesso
            return Ok(new { message = "Logout realizado com sucesso" });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetDto)
        {
            try
            {
                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Email == resetDto.Email);

                if (usuario == null)
                {
                    // Por segurança, sempre retornamos sucesso mesmo se o email não existir
                    return Ok(new { message = "Se o email estiver cadastrado, você receberá as instruções" });
                }

                // Aqui você implementaria o envio de email
                // Por enquanto, apenas simulamos
                
                return Ok(new { message = "Se o email estiver cadastrado, você receberá as instruções" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }
    }
}
