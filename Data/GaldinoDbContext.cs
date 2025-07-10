using Microsoft.EntityFrameworkCore;
using SmartSell.Api.Models.Galdino;

namespace SmartSell.Api.Data
{
    public class GaldinoDbContext : DbContext
    {
        public GaldinoDbContext(DbContextOptions<GaldinoDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Aluno> Alunos { get; set; }
        public DbSet<PontoEmbarque> PontosEmbarque { get; set; }
        public DbSet<Rota> Rotas { get; set; }
        public DbSet<RotaAluno> RotaAlunos { get; set; }
        public DbSet<Pagamento> Pagamentos { get; set; }
        public DbSet<Presenca> Presencas { get; set; }
        public DbSet<Notificacao> Notificacoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurações de enum para MySQL
            modelBuilder.Entity<Usuario>()
                .Property(e => e.Tipo)
                .HasConversion<string>();

            modelBuilder.Entity<Rota>()
                .Property(e => e.Status)
                .HasConversion<string>();

            modelBuilder.Entity<RotaAluno>()
                .Property(e => e.Confirmado)
                .HasConversion<string>();

            modelBuilder.Entity<Pagamento>()
                .Property(e => e.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Pagamento>()
                .Property(e => e.FormaPagamento)
                .HasConversion<string>();

            modelBuilder.Entity<Presenca>()
                .Property(e => e.Presente)
                .HasConversion<string>();

            // Configurações de relacionamentos
            modelBuilder.Entity<Rota>()
                .HasOne(r => r.Motorista)
                .WithMany(u => u.Rotas)
                .HasForeignKey(r => r.FkIdMotorista)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RotaAluno>()
                .HasOne(ra => ra.Rota)
                .WithMany(r => r.RotaAlunos)
                .HasForeignKey(ra => ra.FkIdRota)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RotaAluno>()
                .HasOne(ra => ra.Aluno)
                .WithMany(a => a.RotaAlunos)
                .HasForeignKey(ra => ra.FkIdAluno)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RotaAluno>()
                .HasOne(ra => ra.PontoEmbarque)
                .WithMany(p => p.RotaAlunos)
                .HasForeignKey(ra => ra.FkIdPonto)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pagamento>()
                .HasOne(p => p.Aluno)
                .WithMany(a => a.Pagamentos)
                .HasForeignKey(p => p.FkIdAluno)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Presenca>()
                .HasOne(p => p.Rota)
                .WithMany(r => r.Presencas)
                .HasForeignKey(p => p.FkIdRota)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Presenca>()
                .HasOne(p => p.Aluno)
                .WithMany(a => a.Presencas)
                .HasForeignKey(p => p.FkIdAluno)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Notificacao>()
                .HasOne(n => n.Aluno)
                .WithMany(a => a.Notificacoes)
                .HasForeignKey(n => n.FkIdAluno)
                .OnDelete(DeleteBehavior.Cascade);

            // Índices únicos
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Aluno>()
                .HasIndex(a => a.Cpf)
                .IsUnique();

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Usuários iniciais
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

            // Alunos iniciais
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

            // Pontos de embarque iniciais
            modelBuilder.Entity<PontoEmbarque>().HasData(
                new PontoEmbarque
                {
                    IdPonto = 1,
                    Nome = "Terminal Central",
                    Rua = "Av. Principal, 123",
                    Bairro = "Centro",
                    Cidade = "São Paulo",
                    PontoReferencia = "Próximo ao shopping"
                },
                new PontoEmbarque
                {
                    IdPonto = 2,
                    Nome = "Estação Metro Norte",
                    Rua = "Rua das Flores, 456",
                    Bairro = "Vila Norte",
                    Cidade = "São Paulo",
                    PontoReferencia = "Saída A do metrô"
                }
            );
        }
    }
}
