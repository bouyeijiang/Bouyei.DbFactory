using System;
using System.Data;
using System.Data.Common;

namespace Bouyei.DbFactory.DbAdoProvider.SqlServer
{
    internal class SqlFactory : BaseFactory
    {
        public SqlFactory(string ConnectionString, int timeout)
            : base(FactoryType.SqlServer, timeout, ConnectionString)
        {
           
        }

        public SqlFactory(IDbConnection dbConnection, IDbTransaction dbTransaction,
              int timeout = 1800)
              : base(FactoryType.SqlServer, timeout, dbConnection, dbTransaction)
        {
           
        }
  
        public static DbProviderFactory GetFactory()
        {
            return System.Data.SqlClient.SqlClientFactory.Instance;
        }

        public static DbParameter GetParameter(CmdParameter param)
        {
            var p = new System.Data.SqlClient.SqlParameter(param.ParameterName, param.Value);
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
