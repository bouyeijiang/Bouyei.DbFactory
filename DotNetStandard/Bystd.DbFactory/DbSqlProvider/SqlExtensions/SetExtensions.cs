using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bystd.DbFactory
{
    using DbSqlProvider.SqlKeywords;

    public static class SetExtensions
    {
        public static Set Set(this Update update, Dictionary<string, object> nameValues)
        {
            Set set = new Set();
            set.SqlString = update.SqlString + set.ToString(nameValues);
            return set;
        }

        public static Set<T> Set<T>(this Update<T> update, T value)
        {
            Set<T> set = new Set<T>();
            set.SqlString = update.SqlString + set.ToString(value);
            return set;
        }
        public static Set Set<T>(this Update<T> update, Dictionary<string, object> setKeyValues)
        {
            Set set = new Set();
            set.SqlString = update.SqlString + set.ToString(setKeyValues);
            return set;
        }

    }
}
