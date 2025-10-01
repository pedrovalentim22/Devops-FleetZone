using Microsoft.EntityFrameworkCore;
using MottuCrudAPI.Infrastructure.Mappings;
using DomainEntities = MottuCrudAPI.Domain.Entities; // alias para Domain
using PersistenseEntities = MottuCrudAPI.Persistense; // alias para Persistense

namespace MottuCrudAPI.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Usando entidades do Domain
        public DbSet<DomainEntities.Motocicleta> Motocicletas { get; set; }
        public DbSet<DomainEntities.Patio> Patios { get; set; }
        public DbSet<DomainEntities.Movimentacao> Movimentacoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Ignorar tabelas antigas da Persistense
            modelBuilder.Ignore<PersistenseEntities.Patio>();
            modelBuilder.Ignore<PersistenseEntities.Moto>();

            // Mappings das entidades do Domain
            modelBuilder.ApplyConfiguration(new PatioMapping());
            modelBuilder.ApplyConfiguration(new MotoMapping()); // se esse mapping for da Persistense, substitua pelo do Domain
            modelBuilder.ApplyConfiguration(new MovimentacaoMapping()); // **adicione isso**
        }
    }
}
