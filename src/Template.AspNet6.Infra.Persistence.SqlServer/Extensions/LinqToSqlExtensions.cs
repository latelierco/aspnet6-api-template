using LinqKit;
using System.Linq.Expressions;

namespace Template.AspNet6.Infra.Persistence.SqlServer.Extensions;

public static class LinqToSqlExtensions
{
    public static IQueryable<T> WhereAny<T>(this IQueryable<T> q, params Expression<Func<T, bool>>[] predicates)
    {
        var orPredicate = PredicateBuilder.New<T>();
        foreach (var predicate in predicates)
        {
            orPredicate = orPredicate.Or(predicate);
        }

        return q.AsExpandable().Where(orPredicate);
    }
}
