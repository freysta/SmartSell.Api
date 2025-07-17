using Microsoft.EntityFrameworkCore;
using SmartSell.Api.Data;
using SmartSell.Api.Models.Galdino;

namespace SmartSell.Api.DAO
{
    public class EmergenciaDAO
    {
        private readonly GaldinoDbContext _context;

        public EmergenciaDAO(GaldinoDbContext context)
        {
            _context = context;
        }

        public List<Emergencia> GetAll(string tipo = "")
        {
            List<Emergencia> emergencias = new List<Emergencia>();
            try
            {
                if (string.IsNullOrEmpty(tipo))
                {
                    emergencias = _context.Emergencias.ToList();
                }
                else
                {
                    emergencias = _context.Emergencias
                        .Where(e => e._tipoEmergencia.Contains(tipo))
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return emergencias;
        }

        public Emergencia? GetById(int id)
        {
            try
            {
                return _context.Emergencias
                    .FirstOrDefault(e => e._id == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Emergencia Create(Emergencia emergencia)
        {
            try
            {
                _context.Emergencias.Add(emergencia);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return emergencia;
        }

        public Emergencia Update(Emergencia emergencia)
        {
            try
            {
                _context.Emergencias.Update(emergencia);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return emergencia;
        }

        public bool Delete(int id)
        {
            try
            {
                var emergencia = _context.Emergencias.Find(id);
                if (emergencia != null)
                {
                    _context.Emergencias.Remove(emergencia);
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

        public List<Emergencia> GetByRota(int rotaId)
        {
            try
            {
                return _context.Emergencias
                    .Where(e => e._rotaId == rotaId)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Emergencia> GetNaoResolvidas()
        {
            try
            {
                return _context.Emergencias
                    .Where(e => !e._resolvido)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
