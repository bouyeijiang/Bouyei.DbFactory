using System;
using System.Data;
using System.Data.Common;

namespace Bouyei.DbFactory.DbAdoProvider.Factories
{
    using Bulkcopies;

    internal class Db2Factory:Db2Bulk,IFactory
    {
        public Db2Factory() { }

        public Db2Factory(string ConnectionString, int timeout = 1800, BulkCopyOptions opt = BulkCopyOptions.KeepIdentity)
            : base(ConnectionString, timeout, opt)
        {

        }

        public Db2Factory(IDbConnection dbConnection, int timeout = 1800,
            BulkCopyOptions option = BulkCopyOptions.KeepIdentity)
            :base(dbConnection,timeout,option)
        {

        }

        public override DbProviderFactory GetFactory()
        {
            return IBM.Data.DB2.DB2Factory.Instance;
        }
    }
}
