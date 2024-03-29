﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bystd.DbFactory.DbSqlProvider.SqlFunctions
{
   public class Min:FunctionsBase
    {
        public Min(string columnName)
            : base(columnName) { }

        public override string ToString()
        {
            return string.Format("Min({0}) ", ColumnName);
        }
    }
}
