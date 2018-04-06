using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouyei.DbFactory.DbSqlProvider.SqlExpression
{
    public class From:SqlBase
    {
        public string TableName { get; private set; }

        public From(string tableName)
        {
            this.TableName = tableName;
        }

        public override string ToString()
        {
            return string.Format("From {0}",TableName);
        }

        public virtual string ToString<T>()
        {
            TableName = typeof(T).Name;

            return string.Format("From {0}", TableName);
        }
    }
}
