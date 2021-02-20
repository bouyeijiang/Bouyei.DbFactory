using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bystd.DbFactory.DbSqlProvider.SqlKeywords
{
   public class Insert:WordsBase
    {
        public string TableName { get; private set; }

        public Insert(string tableName)
        {
            this.TableName = tableName;
        }

        public override string ToString(string[] columnNames)
        {
            return string.Format("Insert Into {0}({1}) ", TableName, base.ToString(columnNames));
        }

        public string ToString(Dictionary<string, object> columns)
        {
            var columnNames = columns.Select(x => x.Key).ToArray();

            return string.Format("Insert Into {0}({1}) ", TableName, base.ToString(columnNames));
        }

    }

    public class Insert<T> : WordsBase
    {
        public string TableName { get; private set; }

        public Insert():base(typeof(T),IgnoreType.IgnoreWrite)
        {
            this.TableName = GetTableName();
        }

        public override string ToString()
        {
            var ColumnNames = GetColumns();

            return string.Format("Insert Into {0}({1}) ",TableName, base.ToString(ColumnNames));
        }

        public string ToString(Dictionary<string, object> columns)
        {
            var columnNames = columns.Select(x => x.Key).ToArray();

            return string.Format("Insert Into {0}({1}) ", TableName, base.ToString(columnNames));
        }
    }
}
