using Microsoft.AspNetCore.Mvc;
using SmartSell.Api.DAO;
using SmartSell.Api.Data;
using SmartSell.Api.Models.Galdino;

namespace SmartSell.Api.Controllers.Galdino
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly AlunoDAO _alunoDAO;
        private readonly UsuarioDAO _usuarioDAO;

        public StudentsController(GaldinoDbContext context)
        {
            _alunoDAO = new AlunoDAO(context);
            _usuarioDAO = new UsuarioDAO(context);
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] string? status, [FromQuery] string? route)
        {
            try
            {
                var query = _alunoDAO.GetAll().AsQueryable();

                if (!string.IsNullOrEmpty(status))
                {
                    bool isActive = status.ToLower() == "ativo";
                    query = query.Where(a => a.Usuario != null && a.Usuario._ativo == isActive);
                }

                if (!string.IsNullOrEmpty(route))
                    query = query.Where(a => a.RotaAlunos.Any(ra => ra._rotaId.ToString() == route));

                var alunos = query.ToList().Select(a => new
                {
                    id = a._id,
                    name = a.Usuario?._nome ?? "",
                    email = a.Usuario?._email ?? "",
                    phone = a._telefone,
                    cpf = a._cpf,
                    address = a._endereco,
                    city = a._cidade,
                    course = a._curso,
                    shift = ConvertShiftToFrontend(a._turno),
                    institution = a.Instituicao?._nome,
                    paymentStatus = CalculatePaymentStatus(a._id),
                    route = GetStudentRoute(a._id),
                    enrollmentDate = DateTime.Now.ToString("yyyy-MM-dd"),
                    status = a.Usuario?._ativo == true ? "Ativo" : "Inativo",
                    createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
                }).ToList();

                return Ok(alunos);
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
                var aluno = _alunoDAO.GetById(id);
                if (aluno == null)
                    return NotFound("Aluno não encontrado");

                var response = new
                {
                    id = aluno._id,
                    name = aluno.Usuario?._nome ?? "",
                    email = aluno.Usuario?._email ?? "",
                    phone = aluno._telefone,
                    cpf = aluno._cpf,
                    address = aluno._endereco,
                    city = aluno._cidade,
                    course = aluno._curso,
                    shift = ConvertShiftToFrontend(aluno._turno),
                    institution = aluno.Instituicao?._nome,
                    paymentStatus = CalculatePaymentStatus(aluno._id),
                    route = GetStudentRoute(aluno._id),
                    enrollmentDate = DateTime.Now.ToString("yyyy-MM-dd"),
                    status = aluno.Usuario?._ativo == true ? "Ativo" : "Inativo",
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
        public IActionResult Create([FromBody] CreateStudentRequest request)
        {
            try
            {
                var existingCpf = _alunoDAO.GetByCpf(request.Cpf);
                if (existingCpf != null)
                {
                    return BadRequest(new { message = "CPF já está em uso" });
                }

                var existingEmail = _usuarioDAO.GetByEmail(request.Email);
                if (existingEmail != null)
                {
                    return BadRequest(new { message = "Email já está em uso" });
                }

                if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Email))
                {
                    return BadRequest(new { message = "Nome e email são obrigatórios" });
                }

                var usuario = new Usuario
                {
                    _nome = request.Name,
                    _email = request.Email,
                    _senha = BCrypt.Net.BCrypt.HashPassword("TempPass123!"),
                    _ativo = true
                };

                _usuarioDAO.Create(usuario);

                var aluno = new Aluno
                {
                    _telefone = request.Phone,
                    _cpf = request.Cpf,
                    _endereco = request.Address,
                    _cidade = request.City,
                    _curso = request.Course,
                    _turno = ConvertShiftFromFrontend(request.Shift),
                    _usuarioId = usuario._id,
                    _instituicaoId = 1
                };

                _alunoDAO.Create(aluno);

                var response = new
                {
                    id = aluno._id,
                    name = usuario._nome,
                    email = usuario._email,
                    phone = aluno._telefone,
                    cpf = aluno._cpf,
                    address = aluno._endereco,
                    city = aluno._cidade,
                    course = aluno._curso,
                    shift = ConvertShiftToFrontend(aluno._turno),
                    paymentStatus = "Em dia",
                    route = request.Route,
                    enrollmentDate = DateTime.Now.ToString("yyyy-MM-dd"),
                    status = "Ativo",
                    createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
                };

                return StatusCode(201, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateStudentRequest request)
        {
            try
            {
                var aluno = _alunoDAO.GetById(id);
                if (aluno == null)
                    return NotFound("Aluno não encontrado");


                if (!string.IsNullOrEmpty(request.Cpf) && request.Cpf != aluno._cpf)
                {
                    var existingCpf = _alunoDAO.GetByCpf(request.Cpf);

                    if (existingCpf != null && existingCpf._id != id)
                    {
                        return BadRequest(new { message = "CPF já está em uso" });
                    }
                }

                aluno._telefone = request.Phone ?? aluno._telefone;
                aluno._cpf = request.Cpf ?? aluno._cpf;
                aluno._endereco = request.Address ?? aluno._endereco;
                aluno._cidade = request.City ?? aluno._cidade;
                aluno._curso = request.Course ?? aluno._curso;
                aluno._turno = ConvertShiftFromFrontend(request.Shift) ?? aluno._turno;

                _alunoDAO.Update(aluno);

                var response = new
                {
                    id = aluno._id,
                    name = request.Name ?? aluno.Usuario?._nome ?? "",
                    email = request.Email ?? aluno.Usuario?._email ?? "",
                    phone = aluno._telefone,
                    cpf = aluno._cpf,
                    address = aluno._endereco,
                    city = aluno._cidade,
                    course = aluno._curso,
                    shift = ConvertShiftToFrontend(aluno._turno),
                    paymentStatus = CalculatePaymentStatus(aluno._id),
                    route = GetStudentRoute(aluno._id),
                    enrollmentDate = DateTime.Now.ToString("yyyy-MM-dd"),
                    status = aluno.Usuario?._ativo == true ? "Ativo" : "Inativo",
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
                var aluno = _alunoDAO.GetById(id);
                if (aluno == null)
                    return NotFound("Aluno não encontrado");

                _alunoDAO.Delete(id);
                return Ok(new { message = "Aluno removido com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        private string ConvertShiftToFrontend(TurnoEnum? turno)
        {
            return turno switch
            {
                TurnoEnum.Matutino => "Manha",
                TurnoEnum.Vespertino => "Tarde", 
                TurnoEnum.Noturno => "Noite",
                TurnoEnum.Integral => "Integral",
                _ => ""
            };
        }

        private TurnoEnum? ConvertShiftFromFrontend(string? shift)
        {
            return shift switch
            {
                "Manha" => TurnoEnum.Matutino,
                "Tarde" => TurnoEnum.Vespertino,
                "Noite" => TurnoEnum.Noturno,
                "Integral" => TurnoEnum.Integral,
                _ => null
            };
        }

        private string CalculatePaymentStatus(int alunoId)
        {
            return "Em dia";
        }

        private string? GetStudentRoute(int alunoId)
        {
            return null;
        }
    }

    public class CreateStudentRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Course { get; set; }
        public string? Shift { get; set; }
        public string? Route { get; set; }
        public string? EnrollmentDate { get; set; }
    }

    public class UpdateStudentRequest
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Cpf { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Course { get; set; }
        public string? Shift { get; set; }
        public string? Status { get; set; }
    }
}
