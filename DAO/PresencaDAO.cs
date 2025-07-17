using Microsoft.EntityFrameworkCore;
using SmartSell.Api.Data;
using SmartSell.Api.Models.Galdino;

namespace SmartSell.Api.DAO
{
    public class PresencaDAO
    {
        private readonly GaldinoDbContext _context;

        public PresencaDAO(GaldinoDbContext context)
        {
            _context = context;
        }

        public List<Presenca> GetAll()
        {
            List<Presenca> presencas = new List<Presenca>();
            try
            {
                presencas = _context.Presencas.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return presencas;
        }

        public Presenca? GetById(int id)
        {
            try
            {
                return _context.Presencas
                    .FirstOrDefault(p => p._id == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Presenca Create(Presenca presenca)
        {
            try
            {
                _context.Presencas.Add(presenca);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return presenca;
        }

        public Presenca Update(Presenca presenca)
        {
            try
            {
                _context.Presencas.Update(presenca);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return presenca;
        }

        public bool Delete(int id)
        {
            try
            {
                var presenca = _context.Presencas.Find(id);
                if (presenca != null)
                {
                    _context.Presencas.Remove(presenca);
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
