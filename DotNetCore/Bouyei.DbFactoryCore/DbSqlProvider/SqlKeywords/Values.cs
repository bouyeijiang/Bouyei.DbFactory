using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;

namespace Bouyei.DbFactoryCore.DbSqlProvider.SqlKeywords
{
    public class Values:WordsBase
    {
        public Values( )
        {
            
        }

        public string ToString(IList<object[]> value)
        {
            StringBuilder builder = new StringBuilder("Values ");
            for (int i = 0; i < value.Count; ++i)
            {
                builder.AppendFormat("({0}){1}",
                    base.ParameterFormat(value[i]), i < value.Count - 1 ? "," : "");
            }
            return builder.Append(" ").ToString();
        }
    }

    public class Values<T> : WordsBase
    {
        public Values():base(typeof(T))
        {

        }

        public string ToString(params T[] value)
        {
            StringBuilder builder = new StringBuilder("Values ");
            var pros = GetProperties();

            for (int i = 0; i < value.Length; ++i)
            {
                builder.AppendFormat("({0}){1}",
                    base.ParameterFormat(pros, value[i]), i < value.Length - 1 ? "," : "");
            }
            return builder.Append(" ").ToString();
        }
    }
}
