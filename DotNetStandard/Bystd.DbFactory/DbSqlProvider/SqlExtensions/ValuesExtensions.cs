using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bystd.DbFactory
{
    using DbSqlProvider.SqlKeywords;

    public static class ValuesExtensions
    {
        public static Values Values(this Insert insert, Dictionary<string, object> columns)
        {
            Values val = new Values();
            val.SqlString = insert.SqlString + val.ToString(columns);

            return val;
        }

        public static Values<T> Values<T>(this Insert<T> insert, params T[] values)
        {
            Values<T> val = new Values<T>();
            val.SqlString = insert.SqlString + val.ToString(values);

            return val;
        }

        public static Values<T> Values<T, R>(this Insert<T> insert, Func<T, R> selector)
        {
            Values<T> val = new Values<T>();
            val.SqlString = insert.SqlString + val.ToString(selector);

            return val;
        }

        public static Values<T> Values<T>(this Insert<T> insert, Dictionary<string, object> columns)
        {
            Values<T> val = new Values<T>();
            val.SqlString = insert.SqlString + val.ToString(columns);

            return val;
        }
    }
}
