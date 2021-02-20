using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bystd.DbFactory.DbSqlProvider.SqlKeywords
{
    public class Update : WordsBase
    {
        public Update() { }

        public string ToString(string tableName)
        {
            return string.Format("Update {0} ", tableName);
        }
    }

    public class Update<T> : WordsBase
    {
        public Update():base(typeof(T)) { }

        public override string ToString()
        {
            return string.Format("Update {0} ", GetTableName());
        }
    }
}
