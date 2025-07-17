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
        public DbSet<Instituicao> Instituicoes { get; set; }
        public DbSet<Aluno> Alunos { get; set; }
        public DbSet<Motorista> Motoristas { get; set; }
        public DbSet<GestorSistema> GestoresSistema { get; set; }
        public DbSet<Onibus> Onibus { get; set; }
        public DbSet<PontoEmbarque> PontosEmbarque { get; set; }
        public DbSet<Rota> Rotas { get; set; }
        public DbSet<RotaPonto> RotaPontos { get; set; }
        public DbSet<RotaAluno> RotaAlunos { get; set; }
        public DbSet<Presenca> Presencas { get; set; }
        public DbSet<Pagamento> Pagamentos { get; set; }
        public DbSet<Notificacao> Notificacoes { get; set; }
        public DbSet<Emergencia> Emergencias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u._email)
                .IsUnique();

            modelBuilder.Entity<Aluno>()
                .HasIndex(a => a._cpf)
                .IsUnique();

            modelBuilder.Entity<Motorista>()
                .HasIndex(m => m._cnh)
                .IsUnique();

            modelBuilder.Entity<Motorista>()
                .HasIndex(m => m._cpf)
                .IsUnique();

            modelBuilder.Entity<Onibus>()
                .HasIndex(o => o._placa)
                .IsUnique();

            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
        }
    }
}
