using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouyei.DbFactory.DbSqlProvider.SqlExpression
{
   public class Delete:SqlBase
    {
        public Delete() { }

        public override string ToString()
        {
            return string.Format("Delete ");
        }
    }
}
