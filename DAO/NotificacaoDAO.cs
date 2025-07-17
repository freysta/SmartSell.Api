using Microsoft.EntityFrameworkCore;
using SmartSell.Api.Data;
using SmartSell.Api.Models.Galdino;

namespace SmartSell.Api.DAO
{
    public class NotificacaoDAO
    {
        private readonly GaldinoDbContext _context;

        public NotificacaoDAO(GaldinoDbContext context)
        {
            _context = context;
        }

        public List<Notificacao> GetAll()
        {
            List<Notificacao> notificacoes = new List<Notificacao>();
            try
            {
                notificacoes = _context.Notificacoes.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return notificacoes;
        }

        public Notificacao? GetById(int id)
        {
            try
            {
                return _context.Notificacoes
                    .FirstOrDefault(n => n._id == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Notificacao Create(Notificacao notificacao)
        {
            try
            {
                _context.Notificacoes.Add(notificacao);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return notificacao;
        }

        public Notificacao Update(Notificacao notificacao)
        {
            try
            {
                _context.Notificacoes.Update(notificacao);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return notificacao;
        }

        public bool Delete(int id)
        {
            try
            {
                var notificacao = _context.Notificacoes.Find(id);
                if (notificacao != null)
                {
                    _context.Notificacoes.Remove(notificacao);
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

        public List<Notificacao> GetByType(string type)
        {
            try
            {
                return _context.Notificacoes
                    .Where(n => n._tipo == type)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
