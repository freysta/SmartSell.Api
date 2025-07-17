using Microsoft.EntityFrameworkCore;
using SmartSell.Api.Data;
using SmartSell.Api.Models.Galdino;

namespace SmartSell.Api.DAO
{
    public class RotaAlunoDAO
    {
        private readonly GaldinoDbContext _context;

        public RotaAlunoDAO(GaldinoDbContext context)
        {
            _context = context;
        }

        public List<RotaAluno> GetAll()
        {
            List<RotaAluno> rotaAlunos = new List<RotaAluno>();
            try
            {
                rotaAlunos = _context.RotaAlunos.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return rotaAlunos;
        }

        public RotaAluno? GetById(int id)
        {
            try
            {
                return _context.RotaAlunos
                    .FirstOrDefault(ra => ra._id == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public RotaAluno Create(RotaAluno rotaAluno)
        {
            try
            {
                _context.RotaAlunos.Add(rotaAluno);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return rotaAluno;
        }

        public RotaAluno Update(RotaAluno rotaAluno)
        {
            try
            {
                _context.RotaAlunos.Update(rotaAluno);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return rotaAluno;
        }

        public bool Delete(int id)
        {
            try
            {
                var rotaAluno = _context.RotaAlunos.Find(id);
                if (rotaAluno != null)
                {
                    _context.RotaAlunos.Remove(rotaAluno);
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

        public List<RotaAluno> GetByRota(int rotaId)
        {
            try
            {
                return _context.RotaAlunos
                    .Where(ra => ra._rotaId == rotaId)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<RotaAluno> GetByAluno(int alunoId)
        {
            try
            {
                return _context.RotaAlunos
                    .Where(ra => ra._alunoId == alunoId)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
