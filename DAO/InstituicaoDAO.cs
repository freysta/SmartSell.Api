using Microsoft.EntityFrameworkCore;
using SmartSell.Api.Data;
using SmartSell.Api.Models.Galdino;

namespace SmartSell.Api.DAO
{
    public class InstituicaoDAO
    {
        private readonly GaldinoDbContext _context;

        public InstituicaoDAO(GaldinoDbContext context)
        {
            _context = context;
        }

        public List<Instituicao> GetAll(string nome = "")
        {
            List<Instituicao> instituicoes = new List<Instituicao>();
            try
            {
                if (string.IsNullOrEmpty(nome))
                {
                    instituicoes = _context.Instituicoes.ToList();
                }
                else
                {
                    instituicoes = _context.Instituicoes
                        .Where(i => i._nome.Contains(nome))
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return instituicoes;
        }

        public Instituicao? GetById(int id)
        {
            try
            {
                return _context.Instituicoes
                    .FirstOrDefault(i => i._id == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Instituicao Create(Instituicao instituicao)
        {
            try
            {
                _context.Instituicoes.Add(instituicao);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return instituicao;
        }

        public Instituicao Update(Instituicao instituicao)
        {
            try
            {
                _context.Instituicoes.Update(instituicao);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return instituicao;
        }

        public bool Delete(int id)
        {
            try
            {
                var instituicao = _context.Instituicoes.Find(id);
                if (instituicao != null)
                {
                    _context.Instituicoes.Remove(instituicao);
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
    }
}
