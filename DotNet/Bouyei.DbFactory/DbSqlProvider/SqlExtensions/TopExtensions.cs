using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouyei.DbFactory
{
    using DbSqlProvider.SqlKeywords;

    public static class TopExtensions
    {
        public static Top Top(this From from, FactoryType dbType, int page = 0, int size = 1)
        {
            Top top = new Top(dbType, page,size);
            top.SqlString = from.SqlString + top.ToString();
            return top;
        }

        public static Top Top<T>(this From<T> from, FactoryType dbType, int page = 0, int size = 1)
        {
            Top top = new Top(dbType, page,size);
            top.SqlString = from.SqlString + top.ToString();
            return top;
        }

        public static Top Top(this Where where, FactoryType dbType, int page = 0, int size = 1)
        {
            Top top = new Top(dbType, page,size);
            top.SqlString = where.SqlString + top.ToString();
            return top;
        }

        public static Top Top<T>(this Where<T> where, FactoryType dbType, int page = 0,int size=1)
        {
            Top top = new Top(dbType, page,size);
            top.SqlString = where.SqlString + top.ToString();
            return top;
        }

        public static Top Top<T>(this GroupBy groupBy, FactoryType dbType, int page = 0, int size = 1, params string[] columnNames)
        {
            Top top = new Top(dbType, page,size);

            top.SqlString = groupBy.SqlString + top.ToString();

            return top;
        }
    }
}
