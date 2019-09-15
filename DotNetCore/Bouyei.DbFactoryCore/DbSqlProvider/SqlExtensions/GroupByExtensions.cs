using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouyei.DbFactoryCore
{
    using DbSqlProvider.SqlKeywords;

    public static class GroupByExtensions
    {
        public static GroupBy GroupBy(this From from,params string[] columnNames)
        {
            GroupBy groupby = new GroupBy(columnNames);
            groupby.SqlString = from.SqlString + groupby.ToString();

            return groupby;
        }

        public static GroupBy GroupBy<T>(this From<T> from, params string[] columnNames)
        {
            GroupBy groupby = new GroupBy(columnNames);
            groupby.SqlString = from.SqlString + groupby.ToString();

            return groupby;
        }

        public static GroupBy GroupBy(this Where where,params string[] columnNames)
        {
            GroupBy groupby = new GroupBy(columnNames);
            groupby.SqlString = where.SqlString + groupby.ToString();

            return groupby;
        }
    }
}
