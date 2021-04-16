using System;
namespace Bouyei.DbFactory
{
    using DbSqlProvider.SqlKeywords;
    public static class OrderByExtensions
    {
        public static OrderBy OrderBy(this From from, SortType sType = SortType.Asc, params string[] columnNames)
        {
            OrderBy orderby = new OrderBy(sType, columnNames);
            orderby.SqlString = from.SqlString + orderby.ToString();

            return orderby;
        }

        public static OrderBy OrderBy<T>(this From<T> from, SortType sType = SortType.Asc, params string[] columnNames)
        {
            OrderBy orderby = new OrderBy(sType, columnNames);
            orderby.SqlString = from.SqlString + orderby.ToString();

            return orderby;
        }

        public static OrderBy OrderBy(this Where where, SortType sType = SortType.Asc, params string[] columnNames)
        {
            OrderBy orderby = new OrderBy(sType, columnNames);
            orderby.SqlString = where.SqlString + orderby.ToString();

            return orderby;
        }

        public static OrderBy OrderBy<T>(this Where<T> where, SortType sType = SortType.Asc, params string[] columnNames)
        {
            OrderBy orderby = new OrderBy(sType, columnNames);
            orderby.SqlString = where.SqlString + orderby.ToString();

            return orderby;
        }
    }
}
