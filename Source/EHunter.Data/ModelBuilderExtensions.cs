using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EHunter.Data
{
    internal static class ModelBuilderExtensions
    {
        public static EntityTypeBuilder<TEntity> GenerateId<TEntity>(this EntityTypeBuilder<TEntity> builder)
            where TEntity : class
        {
            const string IdColumnName = "Id";
            builder.Property<int>(IdColumnName)
                .ValueGeneratedOnAdd();
            builder.HasKey(IdColumnName);
            return builder;
        }
    }
}
