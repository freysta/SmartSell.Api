using Microsoft.EntityFrameworkCore;
using SmartSell.Api.Data;
using SmartSell.Api.Models.Galdino;

namespace SmartSell.Api.DAO
{
    public class OnibusDAO
    {
        private readonly GaldinoDbContext _context;

        public OnibusDAO(GaldinoDbContext context)
        {
            _context = context;
        }

        public List<Onibus> GetAll(string placa = "")
        {
            List<Onibus> onibus = new List<Onibus>();
            try
            {
                if (string.IsNullOrEmpty(placa))
                {
                    onibus = _context.Onibus.ToList();
                }
                else
                {
                    onibus = _context.Onibus
                        .Where(o => o._placa.Contains(placa))
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return onibus;
        }

        public Onibus? GetById(int id)
        {
            try
            {
                return _context.Onibus
                    .FirstOrDefault(o => o._id == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Onibus Create(Onibus onibus)
        {
            try
            {
                _context.Onibus.Add(onibus);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return onibus;
        }

        public Onibus Update(Onibus onibus)
        {
            try
            {
                _context.Onibus.Update(onibus);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return onibus;
        }

        public bool Delete(int id)
        {
            try
            {
                var onibus = _context.Onibus.Find(id);
                if (onibus != null)
                {
                    _context.Onibus.Remove(onibus);
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

        public Onibus? GetByPlaca(string placa)
        {
            try
            {
                return _context.Onibus
                    .FirstOrDefault(o => o._placa == placa);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Onibus> GetByStatus(string status)
        {
            try
            {
                return _context.Onibus
                    .Where(o => o._status == status)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
