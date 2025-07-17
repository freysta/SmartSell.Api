using Microsoft.EntityFrameworkCore;
using SmartSell.Api.Data;
using SmartSell.Api.Models.Galdino;

namespace SmartSell.Api.DAO
{
    public class RotaPontoDAO
    {
        private readonly GaldinoDbContext _context;

        public RotaPontoDAO(GaldinoDbContext context)
        {
            _context = context;
        }

        public List<RotaPonto> GetAll()
        {
            List<RotaPonto> rotaPontos = new List<RotaPonto>();
            try
            {
                rotaPontos = _context.RotaPontos.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return rotaPontos;
        }

        public RotaPonto? GetById(int id)
        {
            try
            {
                return _context.RotaPontos
                    .FirstOrDefault(rp => rp._id == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public RotaPonto Create(RotaPonto rotaPonto)
        {
            try
            {
                _context.RotaPontos.Add(rotaPonto);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return rotaPonto;
        }

        public RotaPonto Update(RotaPonto rotaPonto)
        {
            try
            {
                _context.RotaPontos.Update(rotaPonto);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return rotaPonto;
        }

        public bool Delete(int id)
        {
            try
            {
                var rotaPonto = _context.RotaPontos.Find(id);
                if (rotaPonto != null)
                {
                    _context.RotaPontos.Remove(rotaPonto);
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

        public List<RotaPonto> GetByRota(int rotaId)
        {
            try
            {
                return _context.RotaPontos
                    .Where(rp => rp._rotaId == rotaId)
                    .OrderBy(rp => rp._ordem)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<RotaPonto> GetByPonto(int pontoId)
        {
            try
            {
                return _context.RotaPontos
                    .Where(rp => rp._pontoId == pontoId)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
