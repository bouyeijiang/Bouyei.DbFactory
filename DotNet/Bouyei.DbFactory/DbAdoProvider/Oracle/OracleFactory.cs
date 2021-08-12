using System;
using System.Data.Common;
using System.Data;
using Oracle.DataAccess.Client;

namespace Bouyei.DbFactory.DbAdoProvider.Oracle
{

    internal class OracleFactory : BaseFactory, IBaseFactory
    {
        public OracleFactory(string ConnectionString, int timeout = 1800)
            : base(FactoryType.Oracle, timeout, ConnectionString) { }

        public OracleFactory(IDbConnection dbConnection, int timeout = 1800)
             : base(FactoryType.Oracle, timeout, dbConnection, null) { }

        public static DbProviderFactory GetFactory()
        {
            return OracleClientFactory.Instance;
        }
    }
}
