using Habr.Common.Constants;
using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Habr.DataAccess.Configurations
{
    public class DeclarationConfiguration : IEntityTypeConfiguration<Declaration>
    {
        public void Configure(EntityTypeBuilder<Declaration> builder)
        {
            builder.Property(d => d.Id)
                .IsRequired()
                .UseSequence(EntityConfigConsts.DeclarationSequenceName);

            builder.Property(d => d.Text)
                .IsRequired()
                .HasMaxLength(EntityConfigConsts.DeclarationTextMaxLength);

            builder.Property(d => d.CreatedAt)
                .IsRequired();

            builder.Property(d => d.UpdatedAt)
                .IsRequired();

            builder.Property(d => d.CreatorId)
                .IsRequired(false);

            builder.HasMany(d => d.Comments)
                .WithOne(c => c.Parrent)
                .HasForeignKey(c => c.ParrentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.UseTpcMappingStrategy();
        }
    }
}