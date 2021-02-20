using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouyei.DbFactory.DbSqlProvider.SqlKeywords
{
    public class From<T> : WordsBase
    {
        public string TableName { get; private set; }

        public From() : base(typeof(T))
        {
            TableName = GetTableName();
        }

        public override string ToString()
        {
            return "From " + TableName + " ";
        }
    }

    public class From:WordsBase
    {
        public string TableName { get; private set; }

        public From(string tableName)
        {
            this.TableName = tableName;
        }

        public override string ToString()
        {
            return "From " + TableName + " ";
        }
    }
}
