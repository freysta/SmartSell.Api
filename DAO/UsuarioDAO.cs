using Microsoft.EntityFrameworkCore;
using SmartSell.Api.Data;
using SmartSell.Api.Models.Galdino;

namespace SmartSell.Api.DAO
{
    public class UsuarioDAO
    {
        private readonly GaldinoDbContext _context;

        public UsuarioDAO(GaldinoDbContext context)
        {
            _context = context;
        }

        public List<Usuario> GetAll(string nome = "")
        {
            List<Usuario> usuarios = new List<Usuario>();
            try
            {
                if (string.IsNullOrEmpty(nome))
                {
                    usuarios = _context.Usuarios.ToList();
                }
                else
                {
                    usuarios = _context.Usuarios
                        .Where(u => u._nome.Contains(nome))
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return usuarios;
        }

        public Usuario? GetById(int id)
        {
            try
            {
                return _context.Usuarios
                    .FirstOrDefault(u => u._id == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Usuario Create(Usuario usuario)
        {
            try
            {
                _context.Usuarios.Add(usuario);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return usuario;
        }

        public Usuario Update(Usuario usuario)
        {
            try
            {
                _context.Usuarios.Update(usuario);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return usuario;
        }

        public bool Delete(int id)
        {
            try
            {
                var usuario = _context.Usuarios.Find(id);
                if (usuario != null)
                {
                    _context.Usuarios.Remove(usuario);
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

        public Usuario? GetByEmail(string email)
        {
            try
            {
                return _context.Usuarios
                    .FirstOrDefault(u => u._email == email);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
