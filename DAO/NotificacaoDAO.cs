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
                using (var connection = _context.Database.GetDbConnection())
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                            SELECT 
                                id_notificacao, 
                                titulo, 
                                mensagem, 
                                data_envio, 
                                tipo, 
                                lida, 
                                fk_id_aluno 
                            FROM Notificacao";
                        
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var notificacao = new Notificacao
                                {
                                    _id = reader.GetInt32(0),
                                    _titulo = reader.IsDBNull(1) ? null : reader.GetString(1),
                                    _mensagem = reader.GetString(2),
                                    _dataEnvio = reader.GetDateTime(3),
                                    _tipo = reader.GetString(4),
                                    _lida = reader.GetBoolean(5),
                                    _alunoId = reader.IsDBNull(6) ? null : reader.GetInt32(6)
                                };
                                notificacoes.Add(notificacao);
                            }
                        }
                    }
                }
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
                using (var connection = _context.Database.GetDbConnection())
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                            SELECT 
                                id_notificacao, 
                                titulo, 
                                mensagem, 
                                data_envio, 
                                tipo, 
                                lida, 
                                fk_id_aluno 
                            FROM Notificacao 
                            WHERE id_notificacao = @id";
                        
                        var parameter = command.CreateParameter();
                        parameter.ParameterName = "@id";
                        parameter.Value = id;
                        command.Parameters.Add(parameter);
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Notificacao
                                {
                                    _id = reader.GetInt32(0),
                                    _titulo = reader.IsDBNull(1) ? null : reader.GetString(1),
                                    _mensagem = reader.GetString(2),
                                    _dataEnvio = reader.GetDateTime(3),
                                    _tipo = reader.GetString(4),
                                    _lida = reader.GetBoolean(5),
                                    _alunoId = reader.IsDBNull(6) ? null : reader.GetInt32(6)
                                };
                            }
                            return null;
                        }
                    }
                }
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
