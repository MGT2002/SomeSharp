using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Habr.Common.Constants;

namespace Habr.DataAccess.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(u => u.CreatedAt)
                .IsRequired();

            builder.Property(u => u.UpdatedAt)
                .IsRequired();

            builder.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(EntityConfigConsts.UserNameMaxLength);

            builder.Property(u => u.Email)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(EntityConfigConsts.UserEmailMaxLength);

            builder.HasIndex(u => u.Email)
                .IsUnique();

            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(EntityConfigConsts.UserPasswordHashMaxLength);

            builder.Property(u => u.PasswordSalt)
                .IsRequired()
                .HasMaxLength(EntityConfigConsts.UserPasswordSaltMaxLength);

            builder.HasMany(u => u.Posts)
                .WithOne(p => p.Creator)
                .HasForeignKey(p => p.CreatorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.Comments)
                .WithOne(c => c.Creator)
                .HasForeignKey(c => c.CreatorId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
