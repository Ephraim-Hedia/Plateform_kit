using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Platform.Domain.Common;

namespace Platform.Infrastructure.Persistence.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplySoftDeleteQueryFilter(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                continue;
            }

            var method = typeof(ModelBuilderExtensions)
                .GetMethod(nameof(BuildSoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(entityType.ClrType);

            var filter = (LambdaExpression)method.Invoke(null, null)!;

            entityType.SetQueryFilter(filter);
        }
    }

    private static LambdaExpression BuildSoftDeleteFilter<TEntity>()
        where TEntity : class, ISoftDeletable
    {
        Expression<Func<TEntity, bool>> filter = entity => !entity.IsDeleted;
        return filter;
    }
}
