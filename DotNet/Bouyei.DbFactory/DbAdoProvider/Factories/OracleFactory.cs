using System;
using System.Data.Common;
using System.Data;

namespace Bouyei.DbFactory.DbAdoProvider.Factories
{
    using Bulkcopies;

    internal class OracleFactory:OracleBulk,IFactory
    {
        public OracleFactory()
        { }

        public OracleFactory(string ConnectionString, int timeout = 1800,BulkCopyOptions opt=BulkCopyOptions.KeepIdentity)
            : base(ConnectionString,timeout,opt) { }

       public OracleFactory(IDbConnection dbConnection, int timeout = 1800,
            BulkCopyOptions option = BulkCopyOptions.KeepIdentity)
            : base(dbConnection, timeout, option) { }

        public override DbProviderFactory GetFactory()
        {
            return Oracle.DataAccess.Client.OracleClientFactory.Instance;
        }
    }
}
