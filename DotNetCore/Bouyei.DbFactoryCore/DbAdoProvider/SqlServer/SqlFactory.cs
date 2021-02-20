using System;
using System.Data;
using System.Data.Common;

namespace Bouyei.DbFactoryCore.DbAdoProvider.SqlServer
{
    internal class SqlFactory : BaseFactory, IBaseFactory
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
