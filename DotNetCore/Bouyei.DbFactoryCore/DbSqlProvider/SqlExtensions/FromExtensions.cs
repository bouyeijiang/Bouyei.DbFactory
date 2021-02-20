using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouyei.DbFactoryCore
{
    using DbSqlProvider.SqlKeywords;
    using DbSqlProvider.SqlFunctions;

    public static class FromExtensions
    {
        #region select  
        public static From From(this Select select, string tableName)
        {
            From from = new From(tableName);
            from.SqlString = select.SqlString + from.ToString();
            return from;
        }

        public static From<T> From<T>(this Select<T> select)
        {
            From<T> from = new From<T>();
            from.SqlString = select.SqlString + from.ToString();
            return from;
        }

        public static From<T> From<T>(this Select select)
        {
            From<T> from = new From<T>();
            from.SqlString = select.SqlString + from.ToString();
            return from;
        }

        public static From From(this Top top,string tableName)
        {
            if (top.dbType != FactoryType.SqlServer)
                throw new Exception("not supported this expression");

            From from = new From(tableName);
            from.SqlString = top.SqlString + from.ToString();
            return from;
        }

        public static From<T> From<T>(this Top top)
        {
            if (top.dbType != FactoryType.SqlServer)
                throw new Exception("not supported this expression");

            From<T> from = new From<T>();
            from.SqlString = top.SqlString + from.ToString();
            return from;
        }

        #region function
        public static From<T> From<T>(this Count input)
        {
            From<T> from = new From<T>();
            from.SqlString = input.SqlString + from.ToString();
            return from;
        }

        public static From<T> From<T>(this Max input)
        {
            From<T> from = new From<T>();
            from.SqlString = input.SqlString + from.ToString();
            return from;
        }

        public static From<T> From<T>(this Min input)
        {
            From<T> from = new From<T>();
            from.SqlString = input.SqlString + from.ToString();
            return from;
        }

        public static From<T> From<T>(this Sum input)
        {
            From<T> from = new From<T>();
            from.SqlString = input.SqlString + from.ToString();
            return from;
        }

        public static From<T> From<T>(this Avg input)
        {
            From<T> from = new From<T>();
            from.SqlString = input.SqlString + from.ToString();
            return from;
        }
        #endregion

        #endregion

        #region delete 
        public static From From(this Delete delete, string tableName)
        {
            From from = new From(tableName);
            from.SqlString = delete.SqlString + from.ToString();
            return from;
        }

        public static From<T> From<T>(this Delete delete)
        {
            From<T> from = new From<T>();
            from.SqlString = delete.SqlString + from.ToString();
            return from;
        }
        #endregion
    }
}
