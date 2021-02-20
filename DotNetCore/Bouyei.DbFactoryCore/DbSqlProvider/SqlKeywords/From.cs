using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouyei.DbFactoryCore.DbSqlProvider.SqlKeywords
{
    public class From:WordsBase
    {
        public string TableName { get; private set; }

        public From(string tableName)
        {
            this.TableName = tableName;
        }

        public override string ToString()
        {
            return string.Format("From {0} ",TableName);
        }
    }

    public class From<T>:WordsBase
    {
        public string TableName { get; private set; }

        public From():base(typeof(T))
        {
            this.TableName = GetTableName();
        }

        public override string ToString()
        {
            return string.Format("From {0} ", TableName);
        }
    }
}
