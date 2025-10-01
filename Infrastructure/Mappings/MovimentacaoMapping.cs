using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MottuCrudAPI.Domain.Entities;

public class MovimentacaoMapping : IEntityTypeConfiguration<Movimentacao>
{
    public void Configure(EntityTypeBuilder<Movimentacao> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Tipo)
               .IsRequired()
               .HasMaxLength(30);

        builder.Property(m => m.Observacao)
               .HasMaxLength(250);

        builder.HasOne(m => m.Motocicleta)
               .WithMany(mt => mt.Movimentacoes)
               .HasForeignKey(m => m.MotocicletaId)
               .OnDelete(DeleteBehavior.Restrict); // evita cascata

        builder.HasOne(m => m.Patio)
               .WithMany(p => p.Movimentacoes)
               .HasForeignKey(m => m.PatioId)
               .OnDelete(DeleteBehavior.Restrict); // evita multiple cascade paths
    }
}
