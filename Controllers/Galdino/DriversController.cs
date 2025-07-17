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
        private readonly MotoristaDAO _motoristaDAO;
        private readonly UsuarioDAO _usuarioDAO;

        public DriversController(GaldinoDbContext context)
        {
            _motoristaDAO = new MotoristaDAO(context);
            _usuarioDAO = new UsuarioDAO(context);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var motoristas = _motoristaDAO.GetAll()
                    .Select(m => new
                    {
                        id = m._id,
                        name = m.Usuario?._nome ?? "",
                        email = m.Usuario?._email ?? "",
                        phone = m._telefone,
                        cpf = m._cpf,
                        cnh = m._cnh,
                        licenseExpiry = m._vencCnh.ToString("yyyy-MM-dd"),
                        birthDate = m._dataNascimento.ToString("yyyy-MM-dd"),
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
                // Validar se email já existe
                var existingUser = _usuarioDAO.GetByEmail(request.Email);
                if (existingUser != null)
                {
                    return BadRequest(new { message = "Email já está em uso" });
                }

                // Validar se CPF já existe (se fornecido)
                if (!string.IsNullOrEmpty(request.Cpf))
                {
                    var existingCpf = _motoristaDAO.GetByCpf(request.Cpf);
                    if (existingCpf != null)
                    {
                        return BadRequest(new { message = "CPF já está em uso" });
                    }
                }

                // Validar se CNH já existe (se fornecida)
                if (!string.IsNullOrEmpty(request.Cnh))
                {
                    var existingCnh = _motoristaDAO.GetByCnh(request.Cnh);
                    if (existingCnh != null)
                    {
                        return BadRequest(new { message = "CNH já está em uso" });
                    }
                }

                // Validar campos obrigatórios
                if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Email))
                {
                    return BadRequest(new { message = "Nome e email são obrigatórios" });
                }

                // 1. Primeiro criar o usuário
                var usuario = new Usuario
                {
                    _nome = request.Name,
                    _email = request.Email,
                    _senha = BCrypt.Net.BCrypt.HashPassword(request.Password ?? "TempPass123!"),
                    _ativo = true
                };

                _usuarioDAO.Create(usuario);

                // 2. Depois criar o motorista vinculado ao usuário
                var motorista = new Motorista
                {
                    _cnh = request.Cnh ?? "",
                    _cpf = request.Cpf ?? "",
                    _telefone = request.Phone,
                    _vencCnh = !string.IsNullOrEmpty(request.LicenseExpiry) 
                        ? DateTime.Parse(request.LicenseExpiry) 
                        : DateTime.Now.AddYears(5), // Data padrão se não fornecida
                    _dataNascimento = !string.IsNullOrEmpty(request.BirthDate) 
                        ? DateTime.Parse(request.BirthDate) 
                        : DateTime.Now.AddYears(-30), // Data padrão se não fornecida
                    _usuarioId = usuario._id // ✅ Vinculação correta
                };

                _motoristaDAO.Create(motorista);

                var response = new
                {
                    id = motorista._id,
                    name = usuario._nome,
                    email = usuario._email,
                    phone = motorista._telefone,
                    cpf = motorista._cpf,
                    cnh = motorista._cnh,
                    vehicle = request.Vehicle,
                    licenseExpiry = motorista._vencCnh.ToString("yyyy-MM-dd"),
                    birthDate = motorista._dataNascimento.ToString("yyyy-MM-dd"),
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
                var motorista = _motoristaDAO.GetById(id);

                if (motorista == null)
                    return NotFound("Motorista não encontrado");

                // Verificar se há rotas associadas através do relacionamento
                if (motorista.Rotas != null && motorista.Rotas.Any())
                {
                    return BadRequest(new { message = "Não é possível excluir motorista com rotas associadas" });
                }

                _motoristaDAO.Delete(id);
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
        public string? Cpf { get; set; }
        public string? Cnh { get; set; }
        public string? Vehicle { get; set; }
        public string? LicenseExpiry { get; set; }
        public string? BirthDate { get; set; }
    }

    public class UpdateDriverRequest
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Cpf { get; set; }
        public string? Cnh { get; set; }
        public string? Vehicle { get; set; }
        public string? LicenseExpiry { get; set; }
        public string? BirthDate { get; set; }
    }
}
