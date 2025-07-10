using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartSell.Api.Data;
using SmartSell.Api.DTOs;
using SmartSell.Api.Models.Galdino;
using BCrypt.Net;

namespace SmartSell.Api.Controllers.Galdino
{
    [ApiController]
    [Route("api/drivers")]
    [Authorize]
    public class DriversController : ControllerBase
    {
        private readonly GaldinoDbContext _context;

        public DriversController(GaldinoDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DriverDto>>> GetDrivers()
        {
            try
            {
                var drivers = await _context.Usuarios
                    .Where(u => u.Tipo == TipoUsuario.Motorista)
                    .ToListAsync();

                var driverDtos = drivers.Select(d => new DriverDto
                {
                    Id = d.IdUsuario,
                    Name = d.Nome,
                    Email = d.Email,
                    Phone = "Não informado", // Campo não existe no modelo atual
                    Cnh = "Não informado", // Campo não existe no modelo atual
                    Vehicle = "Não informado", // Campo não existe no modelo atual
                    LicenseExpiry = DateTime.Now.AddYears(1), // Valor padrão
                    Status = "active",
                    CreatedAt = DateTime.Now.AddDays(-30) // Valor padrão
                }).ToList();

                return Ok(driverDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DriverDto>> GetDriver(int id)
        {
            try
            {
                var driver = await _context.Usuarios
                    .Where(u => u.Tipo == TipoUsuario.Motorista && u.IdUsuario == id)
                    .FirstOrDefaultAsync();

                if (driver == null)
                {
                    return NotFound(new { message = "Motorista não encontrado" });
                }

                var driverDto = new DriverDto
                {
                    Id = driver.IdUsuario,
                    Name = driver.Nome,
                    Email = driver.Email,
                    Phone = "Não informado",
                    Cnh = "Não informado",
                    Vehicle = "Não informado",
                    LicenseExpiry = DateTime.Now.AddYears(1),
                    Status = "active",
                    CreatedAt = DateTime.Now.AddDays(-30)
                };

                return Ok(driverDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<DriverDto>> CreateDriver([FromBody] CreateDriverDto createDto)
        {
            try
            {
                // Verificar se email já existe
                var existingUser = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Email == createDto.Email);

                if (existingUser != null)
                {
                    return BadRequest(new { message = "Email já cadastrado" });
                }

                // Validar data de vencimento da CNH
                if (createDto.LicenseExpiry <= DateTime.Now)
                {
                    return BadRequest(new { message = "Data de vencimento da CNH deve ser futura" });
                }

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(createDto.Password);

                var driver = new Usuario
                {
                    Nome = createDto.Name,
                    Email = createDto.Email,
                    Senha = hashedPassword,
                    Tipo = TipoUsuario.Motorista
                };

                _context.Usuarios.Add(driver);
                await _context.SaveChangesAsync();

                var driverDto = new DriverDto
                {
                    Id = driver.IdUsuario,
                    Name = driver.Nome,
                    Email = driver.Email,
                    Phone = createDto.Phone,
                    Cnh = createDto.Cnh,
                    Vehicle = createDto.Vehicle,
                    LicenseExpiry = createDto.LicenseExpiry,
                    Status = "active",
                    CreatedAt = DateTime.Now
                };

                return CreatedAtAction(nameof(GetDriver), new { id = driver.IdUsuario }, driverDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateDriver(int id, [FromBody] UpdateDriverDto updateDto)
        {
            try
            {
                var driver = await _context.Usuarios
                    .Where(u => u.Tipo == TipoUsuario.Motorista && u.IdUsuario == id)
                    .FirstOrDefaultAsync();

                if (driver == null)
                {
                    return NotFound(new { message = "Motorista não encontrado" });
                }

                // Verificar se email já existe em outro usuário
                if (!string.IsNullOrEmpty(updateDto.Email) && updateDto.Email != driver.Email)
                {
                    var existingUser = await _context.Usuarios
                        .FirstOrDefaultAsync(u => u.Email == updateDto.Email && u.IdUsuario != id);

                    if (existingUser != null)
                    {
                        return BadRequest(new { message = "Email já cadastrado para outro usuário" });
                    }
                }

                // Validar data de vencimento da CNH se fornecida
                if (updateDto.LicenseExpiry.HasValue && updateDto.LicenseExpiry <= DateTime.Now)
                {
                    return BadRequest(new { message = "Data de vencimento da CNH deve ser futura" });
                }

                // Atualizar campos
                if (!string.IsNullOrEmpty(updateDto.Name))
                    driver.Nome = updateDto.Name;

                if (!string.IsNullOrEmpty(updateDto.Email))
                    driver.Email = updateDto.Email;

                // Nota: Phone, Cnh, Vehicle e LicenseExpiry não podem ser atualizados
                // pois não existem no modelo atual. Seria necessário expandir o modelo Usuario.

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteDriver(int id)
        {
            try
            {
                var driver = await _context.Usuarios
                    .Where(u => u.Tipo == TipoUsuario.Motorista && u.IdUsuario == id)
                    .FirstOrDefaultAsync();

                if (driver == null)
                {
                    return NotFound(new { message = "Motorista não encontrado" });
                }

                // Verificar se o motorista tem rotas associadas
                var hasRoutes = await _context.Rotas
                    .AnyAsync(r => r.FkIdMotorista == id);

                if (hasRoutes)
                {
                    return BadRequest(new { message = "Não é possível excluir motorista com rotas associadas" });
                }

                _context.Usuarios.Remove(driver);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }
    }
}
