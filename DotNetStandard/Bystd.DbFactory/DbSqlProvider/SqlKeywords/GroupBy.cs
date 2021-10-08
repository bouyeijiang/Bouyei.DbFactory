using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bystd.DbFactory.DbSqlProvider.SqlKeywords
{
    public class GroupBy : WordsBase
    {
        public string[] ColumnNames { get; private set; }

        public GroupBy(params string[] columnNames)
            : base(AttributeType.IgnoreRead | AttributeType.Ignore)
        {
            this.ColumnNames = columnNames;
        }

        public override string ToString()
        {
            return string.Format("Group By {0} ", string.Join(",", ColumnNames));
        }
    }

    public class GroupBy<T> : WordsBase
    {
        public GroupBy() : base(typeof(T), AttributeType.IgnoreRead | AttributeType.Ignore)
        {

        }
        public override string ToString()
        {
            var ColumnNames = GetColumns();
            return string.Format("Group By {0} ", base.ToString(ColumnNames));
        }
    }
}
