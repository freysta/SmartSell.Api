using Microsoft.EntityFrameworkCore;
using SmartSell.Api.Data;
using SmartSell.Api.Models.Galdino;

namespace SmartSell.Api.DAO
{
    public class RotaDAO
    {
        private readonly GaldinoDbContext _context;

        public RotaDAO(GaldinoDbContext context)
        {
            _context = context;
        }

        public List<Rota> GetAll()
        {
            List<Rota> rotas = new List<Rota>();
            try
            {
                rotas = _context.Rotas.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return rotas;
        }

        public Rota? GetById(int id)
        {
            try
            {
                return _context.Rotas
                    .FirstOrDefault(r => r._id == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Rota Create(Rota rota)
        {
            try
            {
                _context.Rotas.Add(rota);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return rota;
        }

        public Rota Update(Rota rota)
        {
            try
            {
                _context.Rotas.Update(rota);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return rota;
        }

        public bool Delete(int id)
        {
            try
            {
                var rota = _context.Rotas.Find(id);
                if (rota != null)
                {
                    _context.Rotas.Remove(rota);
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

        public List<Rota> GetByStatus(string status)
        {
            try
            {
                return _context.Rotas
                    .Where(r => r._status == status)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Rota> GetByData(DateTime data)
        {
            try
            {
                return _context.Rotas
                    .Where(r => r._dataRota.Date == data.Date)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
