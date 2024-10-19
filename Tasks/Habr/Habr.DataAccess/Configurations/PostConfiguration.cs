using Habr.Common.Constants;
using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Habr.DataAccess.Configurations
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {        
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(EntityConfigConsts.PostTitleMaxLength);

            builder.Property(p => p.IsPublished)
                .IsRequired();

            builder.Property(p => p.PublicationDate);
        }
    }
}
