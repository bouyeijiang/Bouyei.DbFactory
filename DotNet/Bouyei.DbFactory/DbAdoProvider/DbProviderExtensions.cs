using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Bouyei.DbFactory.DbAdoProvider
{
    public static class DbProviderExtensions
    {
        public static DbResult<List<T>, string> Query<T>(this IDbProvider dbProvider,
            Expression<Func<T, bool>> predicate) where T : class
        {
            ISqlProvider sql = SqlProvider.CreateProvider();
            var commandText = sql.Select<T>().From<T>().Where(predicate).SqlString;
            var rt = dbProvider.Query<T>(new Parameter(commandText));
            if (rt.Info != string.Empty)
                rt.Info = rt.Info + commandText;

            return rt;
        }

        public static DbResult<int, string> Delete<T>(this IDbProvider dbProvider
            , Expression<Func<T, bool>> predicate)
        {
            ISqlProvider sql = SqlProvider.CreateProvider();
            var commandText = sql.Delete().From<T>().Where(predicate).SqlString;
            var rt = dbProvider.ExecuteCmd(new Parameter(commandText));
            if (rt.Info != string.Empty)
                rt.Info = rt.Info + commandText;

            return rt;
        }

        public static DbResult<int, string> Update<T>(this IDbProvider dbProvider,
            T value, Expression<Func<T, bool>> predicate) where T : class
        {
            ISqlProvider sql = SqlProvider.CreateProvider();
            var commandText = sql.Update<T>().Set<T>(value).Where(predicate).SqlString;
            var rt = dbProvider.ExecuteCmd(new Parameter(commandText));

            if (rt.Info != string.Empty)
                rt.Info = rt.Info + commandText;

            return rt;
        }

        public static DbResult<int, string> Insert<T>(this IDbProvider dbProvider, params T[] value) where T : class
        {
            ISqlProvider sql = SqlProvider.CreateProvider();
            var commandText = sql.Insert<T>().Values<T>(value).SqlString;
            var rt = dbProvider.ExecuteCmd(new Parameter(commandText));

            if (rt.Info != string.Empty)
                rt.Info = rt.Info + commandText;

            return rt;
        }
    }
}
