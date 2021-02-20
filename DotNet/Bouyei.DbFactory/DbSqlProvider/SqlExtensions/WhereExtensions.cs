using System;
using System.Linq.Expressions;

namespace Bouyei.DbFactory 
{
    using DbSqlProvider.SqlKeywords;

    public static class WhereExtensions
    {
        public static Where<T> Where<T>(this From<T> from, Expression<Func<T, bool>> expression)
        {
            Where<T> where = new Where<T>();
            where.SqlString = from.SqlString + where.ToString(expression);
            return where;
        }

        public static Where Where<T>(this From from, Expression<Func<T, bool>> expression)
        {
            Where where = new Where();
            where.SqlString = from.SqlString + where.ToString<T>(expression);
            return where;
        }

        public static Where Where<T>(this Set<T> set, Expression<Func<T, bool>> expression)
        {
            Where where = new Where();
            where.SqlString = set.SqlString + where.ToString<T>(expression);
            return where;
        }

        public static Where Where<T>(this Set set, Expression<Func<T, bool>> expression)
        {
            Where where = new Where();
            where.SqlString = set.SqlString + where.ToString<T>(expression);
            return where;
        }
    }
}
