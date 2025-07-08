using Microsoft.EntityFrameworkCore;
using SmartSell.Api.Models;

namespace SmartSell.Api.Data
{
    public class SmartSellDbContext : DbContext
    {
        public SmartSellDbContext(DbContextOptions<SmartSellDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<StatusPedido> StatusPedidos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<CategoriaProduto> CategoriaProdutos { get; set; }
        public DbSet<ProdutoServico> ProdutosServicos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<ItemPedido> ItensPedido { get; set; }
        public DbSet<FormaPagamento> FormasPagamento { get; set; }
        public DbSet<Pagamento> Pagamentos { get; set; }
        public DbSet<Endereco> Enderecos { get; set; }
        public DbSet<EnderecoEntrega> EnderecosEntrega { get; set; }
        public DbSet<Agendamento> Agendamentos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure enum as string for ProdutoServico.Tipo
            modelBuilder.Entity<ProdutoServico>()
                .Property(p => p.Tipo)
                .HasConversion<string>();

            // Configure enum as string for FormaPagamento.AvistaParcelado
            modelBuilder.Entity<FormaPagamento>()
                .Property(f => f.AvistaParcelado)
                .HasConversion<string>();
        }
    }
}
