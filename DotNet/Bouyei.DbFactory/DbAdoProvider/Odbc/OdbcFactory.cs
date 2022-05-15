using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouyei.DbFactory.DbAdoProvider.Odbc
{
    public class OdbcFactory : BaseFactory
    {
        public OdbcFactory(int timeout = 1800)
            : base(FactoryType.MySql, timeout) { }

        public OdbcFactory(string ConnectionString, int timeout = 1800)
        : base(FactoryType.MySql, timeout, ConnectionString) { }

        public OdbcFactory(IDbConnection dbConnection, IDbTransaction dbTransaction, int timeout)
            : base(FactoryType.MySql, timeout, dbConnection, dbTransaction)
        { }

        public static DbProviderFactory GetFactory()
        {
            return System.Data.Odbc.OdbcFactory.Instance;
        }

        public static DbParameter GetParameter(CmdParameter param)
        {
            var p = new System.Data.Odbc.OdbcParameter(param.ParameterName, param.Value);
            p.DbType = param.DbType;
            p.Size = param.Size;
            p.Direction = param.Direction;
            p.SourceColumn = param.SourceColumn;
            p.SourceVersion = param.SourceVersion;
            p.SourceColumnNullMapping = param.SourceColumnNullMapping;
            return p;
        }
    }
}
