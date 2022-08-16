using System;
using System.Collections.Generic;
using System.Text;

namespace Bystd.DbFactory
{
    using DbSqlProvider.SqlKeywords;

    public static class TopExtensions
    {
        public static Top Top(this From from, FactoryType dbType, int page = 0, int size = 1)
        {
            Top top = new Top(dbType, page, size);
            if (dbType == FactoryType.Oracle)
            {
                top.SqlString = top.ToString(from.SqlString);
            }
            else
            {
                top.SqlString = from.SqlString + top.ToString();
            }
            return top;
        }

        public static Top Top<T>(this From<T> from, FactoryType dbType, int page = 0, int size = 1)
        {
            Top top = new Top(dbType, page, size);
            if (dbType == FactoryType.Oracle)
            {
                top.SqlString = top.ToString(from.SqlString);
            }
            else
            {
                top.SqlString = from.SqlString + top.ToString();
            }
            return top;
        }

        public static Top Top(this Where where, FactoryType dbType, int page = 0, int size = 1)
        {
            Top top = new Top(dbType, page, size);
            if (dbType == FactoryType.Oracle)
            {
                top.SqlString = top.ToString(where.SqlString);
            }
            else
            {
                top.SqlString = where.SqlString + top.ToString();
            }
            return top;
        }

        public static Top Top<T>(this Where<T> where, FactoryType dbType, int page = 0, int size = 1)
        {
            Top top = new Top(dbType, page, size);
            if (dbType == FactoryType.Oracle)
            {
                top.SqlString = top.ToString(where.SqlString);
            }
            else
            {
                top.SqlString = where.SqlString + top.ToString();
            }
            return top;
        }

        public static Top Top<T>(this OrderBy orderby, FactoryType dbType, int page = 0, int size = 1)
        {
            Top top = new Top(dbType, page, size);
            if (dbType == FactoryType.Oracle)
            {
                top.SqlString = top.ToString(orderby.SqlString);
            }
            else
            {
                top.SqlString = orderby.SqlString + top.ToString();
            }
            return top;
        }

        public static Top Top<T>(this GroupBy groupBy, FactoryType dbType, int page = 0, int size = 1, params string[] columnNames)
        {
            Top top = new Top(dbType, page, size);

            if (dbType == FactoryType.Oracle)
            {
                top.SqlString = top.ToString(groupBy.SqlString);
            }
            else
            {
                top.SqlString = groupBy.SqlString + top.ToString();
            }

            return top;
        }
    }
}
