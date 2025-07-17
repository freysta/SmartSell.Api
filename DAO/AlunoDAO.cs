using Microsoft.EntityFrameworkCore;
using SmartSell.Api.Data;
using SmartSell.Api.Models.Galdino;

namespace SmartSell.Api.DAO
{
    public class AlunoDAO
    {
        private readonly GaldinoDbContext _context;

        public AlunoDAO(GaldinoDbContext context)
        {
            _context = context;
        }

        public List<Aluno> GetAll(string nome = "")
        {
            List<Aluno> alunos = new List<Aluno>();
            try
            {
                if (string.IsNullOrEmpty(nome))
                {
                    alunos = _context.Alunos
                        .Include(a => a.Usuario)
                        .Include(a => a.Instituicao)
                        .ToList();
                }
                else
                {
                    alunos = _context.Alunos
                        .Include(a => a.Usuario)
                        .Include(a => a.Instituicao)
                        .Where(a => a.Usuario != null && a.Usuario._nome.Contains(nome))
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return alunos;
        }

        public List<Aluno> GetAllByCpf(string cpf = "")
        {
            List<Aluno> alunos = new List<Aluno>();
            try
            {
                if (string.IsNullOrEmpty(cpf))
                {
                    alunos = _context.Alunos
                        .Include(a => a.Usuario)
                        .Include(a => a.Instituicao)
                        .ToList();
                }
                else
                {
                    alunos = _context.Alunos
                        .Include(a => a.Usuario)
                        .Include(a => a.Instituicao)
                        .Where(a => a._cpf.Contains(cpf))
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return alunos;
        }

        public Aluno? GetById(int id)
        {
            try
            {
                return _context.Alunos
                    .FirstOrDefault(a => a._id == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Aluno Create(Aluno aluno)
        {
            try
            {
                _context.Alunos.Add(aluno);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return aluno;
        }

        public Aluno Update(Aluno aluno)
        {
            try
            {
                _context.Alunos.Update(aluno);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return aluno;
        }

        public bool Delete(int id)
        {
            try
            {
                var aluno = _context.Alunos.Find(id);
                if (aluno != null)
                {
                    _context.Alunos.Remove(aluno);
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Aluno? GetByCpf(string cpf)
        {
            try
            {
                return _context.Alunos
                    .FirstOrDefault(a => a._cpf == cpf);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
