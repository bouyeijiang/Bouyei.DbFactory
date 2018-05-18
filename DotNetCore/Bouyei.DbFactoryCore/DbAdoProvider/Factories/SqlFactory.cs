using System;
using System.Data;
using System.Data.Common;

namespace Bouyei.DbFactoryCore.DbAdoProvider.Factories
{
    using Bulkcopies;

   internal class SqlFactory:SqlBulk,IFactory
    {
        public SqlFactory() { }

        public SqlFactory(string ConnectionString, int timeout = 1800,
            BulkCopyOptions option = BulkCopyOptions.KeepIdentity)
            : base(ConnectionString,timeout,option)
        {

        }


        public SqlFactory(IDbConnection dbConnection, IDbTransaction dbTrans = null,
              int timeout = 1800, BulkCopyOptions option = BulkCopyOptions.KeepIdentity)
              : base(dbConnection, dbTrans, timeout, option)
        {

        }

        public override DbProviderFactory GetFactory()
        {
            return System.Data.SqlClient.SqlClientFactory.Instance;
        }
    }
}
