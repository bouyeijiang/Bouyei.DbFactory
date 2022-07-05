using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouyei.DbFactory.DbSqlProvider.SqlKeywords
{
    public class Top:WordsBase
    {
        private int size = 1;
        private int page = 0;

        public FactoryType dbType;

        public Top(FactoryType dbType, int page = 0, int size = 1)
        {
            this.dbType = dbType;
            this.size = size;
            this.page = page;
        }

        public string ToString(string subSqlString)
        {
            int offset = page * size;
            string nSql = $"Select * from(Select bouyei_t.*,rownum as ROW_NUM From ({subSqlString})bouyei_t) Where ROW_NUM>{offset} and ROW_NUM<={offset + size}";
            return nSql;
        }

        public override string ToString()
        {
            if (dbType == FactoryType.SqlServer)
            {
                return $"Offset {page} rows fetch next {size} rows only ";
            }
            else if (dbType == FactoryType.PostgreSQL
               || dbType == FactoryType.SQLite)
            {
                return $"Limit {size} offset {page} ";
            }
            else if (dbType == FactoryType.MySql)
            {
                return $"Limit {page},{size} ";
            }
            else if (dbType == FactoryType.DB2)
            {
                return $"Fetch first {size} rows only ";
            }
            else
                throw new Exception("not supported grammar:" + dbType);
        }
    }
}
