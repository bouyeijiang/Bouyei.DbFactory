using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouyei.DbFactoryCore
{
    using DbSqlProvider.SqlKeywords;

    public static class ValuesExtensions
    {
        public static Values Values(this Insert insert, IList<object[]> values)
        {
            Values val = new Values();
            val.SqlString = insert.SqlString + val.ToString(values);

            return val;
        }

        public static Values<T> Values<T>(this Insert<T> insert,T[] values)
        {
            Values<T> val = new Values<T>();
            val.SqlString = insert.SqlString + val.ToString(values);

            return val;
        }
    }
}
