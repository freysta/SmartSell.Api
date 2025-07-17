using Microsoft.EntityFrameworkCore;
using SmartSell.Api.Data;
using SmartSell.Api.Models.Galdino;

namespace SmartSell.Api.DAO
{
    public class MotoristaDAO
    {
        private readonly GaldinoDbContext _context;

        public MotoristaDAO(GaldinoDbContext context)
        {
            _context = context;
        }

        public List<Motorista> GetAll()
        {
            try
            {
                return _context.Motoristas
                    .Include(m => m.Usuario)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Motorista? GetById(int id)
        {
            try
            {
                return _context.Motoristas
                    .Include(m => m.Usuario)
                    .FirstOrDefault(m => m._id == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Motorista? GetByCpf(string cpf)
        {
            try
            {
                return _context.Motoristas
                    .Include(m => m.Usuario)
                    .FirstOrDefault(m => m._cpf == cpf);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Motorista? GetByCnh(string cnh)
        {
            try
            {
                return _context.Motoristas
                    .Include(m => m.Usuario)
                    .FirstOrDefault(m => m._cnh == cnh);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Motorista Create(Motorista motorista)
        {
            try
            {
                _context.Motoristas.Add(motorista);
                _context.SaveChanges();
                return motorista;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Motorista Update(Motorista motorista)
        {
            try
            {
                _context.Motoristas.Update(motorista);
                _context.SaveChanges();
                return motorista;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool Delete(int id)
        {
            try
            {
                var motorista = _context.Motoristas.Find(id);
                if (motorista != null)
                {
                    _context.Motoristas.Remove(motorista);
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
