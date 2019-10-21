using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Bouyei.DbFactoryCore.DbAdoProvider.Factories
{
    using Bulkcopies;
    internal class OracleFactory : OracleBulk, IFactory
    {
        public OracleFactory()
        { }
        public OracleFactory(string ConnectionString, int timeout = 1800,
              BulkCopyOptions option = BulkCopyOptions.KeepIdentity)
              : base(ConnectionString, timeout, option)
        {

        }


        public OracleFactory(IDbConnection dbConnection, IDbTransaction dbTrans = null,
              int timeout = 1800, BulkCopyOptions option = BulkCopyOptions.KeepIdentity)
              : base(dbConnection, timeout, option)
        {

        }

        public override DbProviderFactory GetFactory()
        {
            return Oracle.ManagedDataAccess.Client.OracleClientFactory.Instance;
        }
    }
}
