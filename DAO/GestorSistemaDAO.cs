using Microsoft.EntityFrameworkCore;
using SmartSell.Api.Data;
using SmartSell.Api.Models.Galdino;

namespace SmartSell.Api.DAO
{
    public class GestorSistemaDAO
    {
        private readonly GaldinoDbContext _context;

        public GestorSistemaDAO(GaldinoDbContext context)
        {
            _context = context;
        }

        public List<GestorSistema> GetAll()
        {
            List<GestorSistema> gestores = new List<GestorSistema>();
            try
            {
                gestores = _context.GestoresSistema.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return gestores;
        }

        public GestorSistema? GetById(int id)
        {
            try
            {
                return _context.GestoresSistema
                    .FirstOrDefault(g => g._id == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public GestorSistema Create(GestorSistema gestor)
        {
            try
            {
                _context.GestoresSistema.Add(gestor);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return gestor;
        }

        public GestorSistema Update(GestorSistema gestor)
        {
            try
            {
                _context.GestoresSistema.Update(gestor);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return gestor;
        }

        public bool Delete(int id)
        {
            try
            {
                var gestor = _context.GestoresSistema.Find(id);
                if (gestor != null)
                {
                    _context.GestoresSistema.Remove(gestor);
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

        public GestorSistema? GetByUsuario(int usuarioId)
        {
            try
            {
                return _context.GestoresSistema
                    .FirstOrDefault(g => g._usuarioId == usuarioId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<GestorSistema> GetByNivelAcesso(int nivelAcesso)
        {
            try
            {
                return _context.GestoresSistema
                    .Where(g => g._nivelAcesso == nivelAcesso)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
