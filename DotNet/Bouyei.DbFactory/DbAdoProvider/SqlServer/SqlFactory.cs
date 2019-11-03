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
    }
}
