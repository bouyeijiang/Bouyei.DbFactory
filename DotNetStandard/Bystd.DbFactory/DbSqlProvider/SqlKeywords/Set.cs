using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bystd.DbFactory.DbSqlProvider.SqlKeywords
{
    public class Set : WordsBase
    {
        public Set() { }

        public string ToString(Dictionary<string, object> NameValues)
        {
            StringBuilder builder = new StringBuilder("Set ");
            int c = NameValues.Count;

            foreach (var item in NameValues)
            {
                builder.AppendFormat("{0}={1}{2}", item.Key,
                    IsDigital(item.Value) ? item.Value : string.Format("'{0}'", item.Value),
                   (--c) > 0 ? "," : "");
            }
            return builder.Append(" ").ToString();
        }
    }

    public class Set<T> : WordsBase
    {
        public Set():base(typeof(T),IgnoreType.IgnoreWrite) { }

        public string ToString(T value)
        {
            var items =GetProperties();

            List<string> tmp = new List<string>(items.Count());

            foreach (var item in items)
            {
                var rVal = item.GetValue(value, null);
                if (rVal == null) continue;

                tmp.Add(string.Format("{0}={1}", item.Name, IsDigital(rVal) ? rVal : string.Format("'{0}'", rVal)));
            }
            return "Set " + string.Join(",", tmp)+" ";
        }
    }
}
