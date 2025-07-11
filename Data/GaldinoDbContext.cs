using Microsoft.EntityFrameworkCore;
using SmartSell.Api.Models.Galdino;

namespace SmartSell.Api.Data
{
    public class GaldinoDbContext : DbContext
    {
        public GaldinoDbContext(DbContextOptions<GaldinoDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Aluno> Alunos { get; set; }
        public DbSet<Rota> Rotas { get; set; }
        public DbSet<Pagamento> Pagamentos { get; set; }
        public DbSet<Notificacao> Notificacoes { get; set; }
        public DbSet<PontoEmbarque> PontosEmbarque { get; set; }
        public DbSet<Presenca> Presencas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u._email)
                .IsUnique();

            modelBuilder.Entity<Aluno>()
                .HasIndex(a => a._cpf)
                .IsUnique();

            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    _id = 1,
                    _nome = "Admin Galdino",
                    _email = "admin@test.com",
                    _senha = BCrypt.Net.BCrypt.HashPassword("123456"),
                    _tipo = "Admin"
                },
                new Usuario
                {
                    _id = 2,
                    _nome = "Carlos Santos Silva",
                    _email = "motorista@test.com",
                    _senha = BCrypt.Net.BCrypt.HashPassword("123456"),
                    _tipo = "Motorista"
                }
            );

            modelBuilder.Entity<Aluno>().HasData(
                new Aluno
                {
                    _id = 1,
                    _nome = "Ana Silva Santos",
                    _email = "aluno@test.com",
                    _telefone = "(11) 99999-1111",
                    _cpf = "123.456.789-01"
                },
                new Aluno
                {
                    _id = 2,
                    _nome = "João Pedro Oliveira",
                    _email = "joao.pedro@email.com",
                    _telefone = "(11) 99999-2222",
                    _cpf = "987.654.321-00"
                }
            );

            modelBuilder.Entity<Rota>().HasData(
                new Rota
                {
                    _id = 1,
                    _dataRota = DateTime.Now,
                    _destino = "Campus Norte",
                    _horarioSaida = new TimeSpan(8, 0, 0),
                    _status = "Planejada",
                    _fkIdMotorista = 2
                }
            );

            // Seed Pagamentos
            modelBuilder.Entity<Pagamento>().HasData(
                new Pagamento
                {
                    _id = 1,
                    _studentId = 1,
                    _amount = 150.0m,
                    _month = "2024-01",
                    _year = 2024,
                    _status = "paid",
                    _paymentMethod = "PIX",
                    _paymentDate = DateTime.Parse("2024-01-05T10:00:00Z"),
                    _dueDate = DateTime.Parse("2024-01-10T23:59:59Z"),
                    _createdAt = DateTime.Parse("2024-01-01T10:00:00Z")
                }
            );

            // Seed Notificações
            modelBuilder.Entity<Notificacao>().HasData(
                new Notificacao
                {
                    _id = 1,
                    _title = "Pagamento em atraso",
                    _message = "João Silva - Mensalidade de Janeiro",
                    _type = "warning",
                    _priority = "high",
                    _targetType = "specific",
                    _targetIds = "[1]",
                    _createdAt = DateTime.Parse("2024-01-15T10:00:00Z"),
                    _readBy = "[]"
                }
            );

            // Seed Pontos de Embarque
            modelBuilder.Entity<PontoEmbarque>().HasData(
                new PontoEmbarque
                {
                    _id = 1,
                    _name = "Terminal Central",
                    _address = "Av. Principal, 123",
                    _neighborhood = "Centro",
                    _city = "São Paulo",
                    _lat = -23.5505,
                    _lng = -46.6333,
                    _status = "active",
                    _routes = "[1, 2]",
                    _createdAt = DateTime.Parse("2024-01-01T10:00:00Z")
                }
            );

            // Seed Presenças
            modelBuilder.Entity<Presenca>().HasData(
                new Presenca
                {
                    _id = 1,
                    _routeId = 1,
                    _studentId = 1,
                    _status = "present",
                    _observation = "Embarcou no horário",
                    _date = DateTime.Parse("2024-01-15T08:00:00Z"),
                    _createdAt = DateTime.Parse("2024-01-15T08:00:00Z")
                }
            );
        }
    }
}
