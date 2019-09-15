using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouyei.DbFactory.DbSqlProvider.SqlKeywords
{
    public class Select<T>:WordsBase
    {
        public Select() : base(typeof(T))
        {
            
        }

        public override string ToString()
        {
            var columnNames = GetColumns();
            return string.Format("Select {0} ", base.ToString(columnNames));
        }
    }

    public class Select:WordsBase
    {
        public string[] ColumnNames { get; private set; }

        public Select() { }

        public Select(string[] columnNames)
        {
            this.ColumnNames = columnNames;
        }

        public override string ToString()
        {
            return string.Format("Select {0} ", base.ToString(ColumnNames));
        }
    }
}
