using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouyei.DbFactory
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

        public static GroupBy<T> GroupBy<T,R>(this From<T> from, Func<T,R> selector)
        {
            GroupBy<T> groupby = new GroupBy<T>();
            groupby.SqlString = from.SqlString + groupby.ToString(selector);

            return groupby;
        }

        public static GroupBy GroupBy(this Where where,params string[] columnNames)
        {
            GroupBy groupby = new GroupBy(columnNames);
            groupby.SqlString = where.SqlString + groupby.ToString();

            return groupby;
        }

        public static GroupBy<T> GroupBy<T>(this Where where)
        {
            GroupBy<T> groupby = new GroupBy<T>();
            groupby.SqlString = where.SqlString + groupby.ToString();

            return groupby;
        }

        public static GroupBy<T> GroupBy<T>(this Where<T> where)
        {
            GroupBy<T> groupby = new GroupBy<T>();
            groupby.SqlString = where.SqlString + groupby.ToString();

            return groupby;
        }
        public static GroupBy<T> GroupBy<T,R>(this Where<T> where,Func<T,R>selector)
        {
            GroupBy<T> groupby = new GroupBy<T>();
            groupby.SqlString = where.SqlString + groupby.ToString(selector);

            return groupby;
        }

        public static GroupBy GroupBy<T>(this Where<T> where, params string[] columnNames)
        {
            GroupBy groupby = new GroupBy(columnNames);
            groupby.SqlString = where.SqlString + groupby.ToString();

            return groupby;
        }
    }
}
