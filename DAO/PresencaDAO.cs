using Microsoft.EntityFrameworkCore;
using SmartSell.Api.Data;
using SmartSell.Api.Models.Galdino;

namespace SmartSell.Api.DAO
{
    public class PresencaDAO
    {
        private readonly GaldinoDbContext _context;

        public PresencaDAO(GaldinoDbContext context)
        {
            _context = context;
        }

        public List<Presenca> GetAll()
        {
            List<Presenca> presencas = new List<Presenca>();
            try
            {
                using (var connection = _context.Database.GetDbConnection())
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                            SELECT 
                                id_presenca, 
                                fk_id_rota, 
                                fk_id_aluno, 
                                fk_id_ponto, 
                                presente, 
                                horario_embarque, 
                                horario_desembarque, 
                                observacao 
                            FROM Presenca";
                        
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var presenca = new Presenca
                                {
                                    _id = reader.GetInt32(0),
                                    _rotaId = reader.GetInt32(1),
                                    _alunoId = reader.GetInt32(2),
                                    _pontoId = reader.GetInt32(3),
                                    _presente = reader.GetString(4),
                                    _horarioEmbarque = reader.IsDBNull(5) ? null : (TimeSpan)reader.GetValue(5),
                                    _horarioDesembarque = reader.IsDBNull(6) ? null : (TimeSpan)reader.GetValue(6),
                                    _observacao = reader.IsDBNull(7) ? null : reader.GetString(7)
                                };
                                presencas.Add(presenca);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return presencas;
        }

        public Presenca? GetById(int id)
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
                                id_presenca, 
                                fk_id_rota, 
                                fk_id_aluno, 
                                fk_id_ponto, 
                                presente, 
                                horario_embarque, 
                                horario_desembarque, 
                                observacao 
                            FROM Presenca 
                            WHERE id_presenca = @id";
                        
                        var parameter = command.CreateParameter();
                        parameter.ParameterName = "@id";
                        parameter.Value = id;
                        command.Parameters.Add(parameter);
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Presenca
                                {
                                    _id = reader.GetInt32(0),
                                    _rotaId = reader.GetInt32(1),
                                    _alunoId = reader.GetInt32(2),
                                    _pontoId = reader.GetInt32(3),
                                    _presente = reader.GetString(4),
                                    _horarioEmbarque = reader.IsDBNull(5) ? null : (TimeSpan)reader.GetValue(5),
                                    _horarioDesembarque = reader.IsDBNull(6) ? null : (TimeSpan)reader.GetValue(6),
                                    _observacao = reader.IsDBNull(7) ? null : reader.GetString(7)
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

        public Presenca Create(Presenca presenca)
        {
            try
            {
                _context.Presencas.Add(presenca);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return presenca;
        }

        public Presenca Update(Presenca presenca)
        {
            try
            {
                _context.Presencas.Update(presenca);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return presenca;
        }

        public bool Delete(int id)
        {
            try
            {
                var presenca = _context.Presencas.Find(id);
                if (presenca != null)
                {
                    _context.Presencas.Remove(presenca);
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
