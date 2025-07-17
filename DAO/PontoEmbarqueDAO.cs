using Microsoft.EntityFrameworkCore;
using SmartSell.Api.Data;
using SmartSell.Api.Models.Galdino;

namespace SmartSell.Api.DAO
{
    public class PontoEmbarqueDAO
    {
        private readonly GaldinoDbContext _context;

        public PontoEmbarqueDAO(GaldinoDbContext context)
        {
            _context = context;
        }

        public List<PontoEmbarque> GetAll()
        {
            List<PontoEmbarque> pontos = new List<PontoEmbarque>();
            try
            {
                pontos = _context.PontosEmbarque.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return pontos;
        }

        public PontoEmbarque? GetById(int id)
        {
            try
            {
                return _context.PontosEmbarque
                    .FirstOrDefault(p => p._id == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public PontoEmbarque Create(PontoEmbarque ponto)
        {
            try
            {
                _context.PontosEmbarque.Add(ponto);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return ponto;
        }

        public PontoEmbarque Update(PontoEmbarque ponto)
        {
            try
            {
                _context.PontosEmbarque.Update(ponto);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return ponto;
        }

        public bool Delete(int id)
        {
            try
            {
                var ponto = _context.PontosEmbarque.Find(id);
                if (ponto != null)
                {
                    _context.PontosEmbarque.Remove(ponto);
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
