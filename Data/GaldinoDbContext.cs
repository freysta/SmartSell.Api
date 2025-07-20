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

            modelBuilder.Entity<Aluno>()
                .Property(a => a._turno)
                .HasConversion<string>();

            modelBuilder.Entity<Rota>()
                .Property(r => r._tipoRota)
                .HasConversion<string>();

            modelBuilder.Entity<Rota>()
                .Property(r => r._status)
                .HasConversion(
                    v => v.ToString(),
                    v => ConvertStringToStatusRotaEnum(v)
                );

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

            modelBuilder.Entity<Emergencia>(entity =>
            {
                entity.ToTable("emergencia");
                entity.HasKey(e => e._id);
                entity.Property(e => e._id).HasColumnName("id_emergencia");
                entity.Property(e => e._rotaId).HasColumnName("fk_id_rota");
                entity.Property(e => e._tipoEmergencia).HasColumnName("tipo_emergencia");
                entity.Property(e => e._descricao).HasColumnName("descricao");
                entity.Property(e => e._dataHora).HasColumnName("data_hora");
                entity.Property(e => e._resolvido).HasColumnName("resolvido");
                entity.Property(e => e._observacoes).HasColumnName("observacoes");
            });

            modelBuilder.Entity<RotaAluno>(entity =>
            {
                entity.ToTable("rotaalunos");
                entity.HasKey(ra => ra._id);
                entity.Property(ra => ra._id).HasColumnName("id_rota_aluno");
                entity.Property(ra => ra._rotaId).HasColumnName("fk_id_rota");
                entity.Property(ra => ra._alunoId).HasColumnName("fk_id_aluno");
                entity.Property(ra => ra._pontoId).HasColumnName("fk_id_ponto");
                entity.Property(ra => ra._confirmado).HasColumnName("confirmado");
                entity.Property(ra => ra._dataConfirmacao).HasColumnName("data_confirmacao");
            });

            SeedData(modelBuilder);
        }

        private static StatusRotaEnum ConvertStringToStatusRotaEnum(string value)
        {
            return value switch
            {
                "Em andamento" => StatusRotaEnum.EmAndamento,
                "Planejada" => StatusRotaEnum.Planejada,
                "Concluida" => StatusRotaEnum.Concluida,
                "Cancelada" => StatusRotaEnum.Cancelada,
                _ => Enum.TryParse<StatusRotaEnum>(value, true, out var result) ? result : StatusRotaEnum.Planejada
            };
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
        }
    }
}
