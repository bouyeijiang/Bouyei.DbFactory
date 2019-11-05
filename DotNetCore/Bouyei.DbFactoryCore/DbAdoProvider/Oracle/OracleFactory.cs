using System;
using System.Data.Common;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace Bouyei.DbFactoryCore.DbAdoProvider.Oracle
{
    internal class OracleFactory : BaseFactory
    {
        public OracleFactory(string ConnectionString, int timeout = 1800)
            : base(FactoryType.Oracle, timeout, ConnectionString) { }

        public OracleFactory(IDbConnection dbConnection,IDbTransaction dbTransaction, int timeout = 1800)
             : base(FactoryType.Oracle, timeout, dbConnection, dbTransaction) { }

        public static  DbProviderFactory GetFactory()
        {
            return OracleClientFactory.Instance;
        }
    }
}
