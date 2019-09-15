using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouyei.DbFactory
{
    using DbSqlProvider.SqlKeywords;

    public static class FromExtensions
    {
        #region select  
        public static From From<T>(this Select<T> select, string tableName)
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
