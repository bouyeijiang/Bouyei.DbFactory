using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouyei.DbFactory.DbSqlProvider.SqlFunctions
{
  public  class FunctionsBase
    {
        protected string ColumnName { get; set; }

        public string SqlString { get; set; }

        public FunctionsBase()
        {

        }

        public FunctionsBase(string columnName)
        {
            this.ColumnName = columnName;
        }
    }
}
