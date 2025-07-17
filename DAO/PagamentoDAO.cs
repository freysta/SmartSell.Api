using Microsoft.EntityFrameworkCore;
using SmartSell.Api.Data;
using SmartSell.Api.Models.Galdino;

namespace SmartSell.Api.DAO
{
    public class PagamentoDAO
    {
        private readonly GaldinoDbContext _context;

        public PagamentoDAO(GaldinoDbContext context)
        {
            _context = context;
        }

        public List<Pagamento> GetAll()
        {
            List<Pagamento> pagamentos = new List<Pagamento>();
            try
            {
                pagamentos = _context.Pagamentos.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return pagamentos;
        }

        public Pagamento? GetById(int id)
        {
            try
            {
                return _context.Pagamentos
                    .FirstOrDefault(p => p._id == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Pagamento Create(Pagamento pagamento)
        {
            try
            {
                _context.Pagamentos.Add(pagamento);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return pagamento;
        }

        public Pagamento Update(Pagamento pagamento)
        {
            try
            {
                _context.Pagamentos.Update(pagamento);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return pagamento;
        }

        public bool Delete(int id)
        {
            try
            {
                var pagamento = _context.Pagamentos.Find(id);
                if (pagamento != null)
                {
                    _context.Pagamentos.Remove(pagamento);
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

        public List<Pagamento> GetByAluno(int alunoId)
        {
            try
            {
                return _context.Pagamentos
                    .Where(p => p._alunoId == alunoId)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Pagamento> GetByStatus(string status)
        {
            try
            {
                return _context.Pagamentos
                    .Where(p => p._status == status)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
