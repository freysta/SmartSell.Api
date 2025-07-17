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

            // Configurar relacionamentos
            modelBuilder.Entity<Aluno>()
                .HasOne(a => a.Usuario)
                .WithMany()
                .HasForeignKey(a => a._usuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Aluno>()
                .HasOne(a => a.Instituicao)
                .WithMany()
                .HasForeignKey(a => a._instituicaoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Motorista>()
                .HasOne(m => m.Usuario)
                .WithMany()
                .HasForeignKey(m => m._usuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GestorSistema>()
                .HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(g => g._usuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Rota>()
                .HasOne(r => r.Motorista)
                .WithMany(m => m.Rotas)
                .HasForeignKey(r => r._motoristaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Rota>()
                .HasOne(r => r.Onibus)
                .WithMany(o => o.Rotas)
                .HasForeignKey(r => r._onibusId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Rota>()
                .HasOne(r => r.Instituicao)
                .WithMany()
                .HasForeignKey(r => r._instituicaoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pagamento>()
                .HasOne(p => p.Aluno)
                .WithMany(a => a.Pagamentos)
                .HasForeignKey(p => p._alunoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notificacao>()
                .HasOne<Aluno>()
                .WithMany()
                .HasForeignKey(n => n._alunoId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configurar conversões de enum para string
            modelBuilder.Entity<Aluno>()
                .Property(a => a._turno)
                .HasConversion<string>();

            modelBuilder.Entity<Rota>()
                .Property(r => r._tipoRota)
                .HasConversion<string>();

            modelBuilder.Entity<Rota>()
                .Property(r => r._status)
                .HasConversion<string>();

            modelBuilder.Entity<Pagamento>()
                .Property(p => p._status)
                .HasConversion<string>();

            modelBuilder.Entity<Pagamento>()
                .Property(p => p._formaPagamento)
                .HasConversion<string>();

            modelBuilder.Entity<PontoEmbarque>()
                .Property(pe => pe._tipoPonto)
                .HasConversion<string>();

            modelBuilder.Entity<Onibus>()
                .Property(o => o._status)
                .HasConversion<string>();

            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
        }
    }
}
