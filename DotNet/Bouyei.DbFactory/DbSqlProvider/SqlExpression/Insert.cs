using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouyei.DbFactory.DbSqlProvider.SqlExpression
{
   public class Insert:SqlBase
    {
        public string TableName { get; private set; }

        public string[] ColumnNames { get; private set; }

        public Insert(string tableName, string[] columnNames)
        {
            this.TableName = tableName;
            this.ColumnNames = columnNames;
        }

        public override string ToString()
        {
            return string.Format("Insert Into {0}({1}) ", TableName, base.ToString(ColumnNames));
        }

        public string ToString<T>()
        {
            ColumnNames = ToColumns<T>();

            return string.Format("Insert Into {0}({1}) ", TableName, base.ToString(ColumnNames));
        }
    }
}
