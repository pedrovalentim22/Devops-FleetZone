using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MottuCrudAPI.Domain.Entities;

namespace MottuCrudAPI.Infrastructure.Mappings
{
    public class MotoMapping : IEntityTypeConfiguration<Motocicleta>
    {
        public void Configure(EntityTypeBuilder<Motocicleta> builder)
        {
            builder.ToTable("MOTO");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Placa)
                   .IsRequired()
                   .HasMaxLength(8); // Mercosul

            builder.Property(m => m.Modelo)
                   .IsRequired()
                   .HasMaxLength(80);

            builder.Property(m => m.Status)
                   .HasMaxLength(20)
                   .IsRequired(false);

            builder.Property(m => m.PatioId)
                   .IsRequired();

            // relacionamento com Patio
            builder.HasOne(m => m.Patio)
                   .WithMany(p => p.Motocicletas)
                   .HasForeignKey(m => m.PatioId)
                   .OnDelete(DeleteBehavior.Restrict); // evita cascade
        }
    }
}
