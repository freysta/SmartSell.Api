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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>()
                .Property(e => e.Tipo)
                .HasConversion<string>();

            modelBuilder.Entity<Rota>()
                .Property(e => e.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Aluno>()
                .HasIndex(a => a.Cpf)
                .IsUnique();

            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    IdUsuario = 1,
                    Nome = "Admin Galdino",
                    Email = "admin@test.com",
                    Senha = BCrypt.Net.BCrypt.HashPassword("123456"),
                    Tipo = TipoUsuario.Admin
                },
                new Usuario
                {
                    IdUsuario = 2,
                    Nome = "Carlos Santos Silva",
                    Email = "motorista@test.com",
                    Senha = BCrypt.Net.BCrypt.HashPassword("123456"),
                    Tipo = TipoUsuario.Motorista
                }
            );

            modelBuilder.Entity<Aluno>().HasData(
                new Aluno
                {
                    IdAluno = 1,
                    Nome = "Ana Silva Santos",
                    Email = "aluno@test.com",
                    Telefone = "(11) 99999-1111",
                    Cpf = "123.456.789-01"
                },
                new Aluno
                {
                    IdAluno = 2,
                    Nome = "João Pedro Oliveira",
                    Email = "joao.pedro@email.com",
                    Telefone = "(11) 99999-2222",
                    Cpf = "987.654.321-00"
                }
            );

            modelBuilder.Entity<Rota>().HasData(
                new Rota
                {
                    IdRota = 1,
                    DataRota = DateTime.Now,
                    Destino = "Campus Norte",
                    HorarioSaida = new TimeSpan(8, 0, 0),
                    Status = StatusRota.Planejada,
                    FkIdMotorista = 2
                }
            );
        }
    }
}
