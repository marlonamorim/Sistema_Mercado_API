using Microsoft.EntityFrameworkCore;
using APIProdutos.Models;

namespace APIProdutos.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) :
            base(options) { }

        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Produto>(entity => {
                entity.ToTable("Product");
                entity.HasKey(p => p.CodigoBarras);
                entity.Property(e => e.Imagem).HasColumnName("Image").IsRequired();
                entity.Property(e => e.Nome).HasColumnName("Name").IsRequired();
                entity.Property(e => e.Preco).HasColumnName("Price").IsRequired();
            });

            modelBuilder.Entity<Usuario>(entity => {
                entity.ToTable("User");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Id).HasColumnName("Id");
                entity.Property(p => p.Nome).HasColumnName("Username");
                entity.Property(p => p.ChaveAcesso).HasColumnName("Password");
            });
        }
    }
}