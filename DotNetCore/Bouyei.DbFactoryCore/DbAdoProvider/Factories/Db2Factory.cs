using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Bouyei.DbFactoryCore.DbAdoProvider.Factories
{
    using Bulkcopies;
   public class Db2Factory : Db2Bulk, IFactory
    {
        public Db2Factory()
        { }

        public Db2Factory(string ConnectionString, int timeout = 1800, BulkCopyOptions opt = BulkCopyOptions.KeepIdentity)
           : base(ConnectionString, timeout, opt)
        {

        }

        public Db2Factory(IDbConnection dbConnection, int timeout = 1800,
            BulkCopyOptions option = BulkCopyOptions.KeepIdentity)
            : base(dbConnection, timeout, option)
        {

        }

        public override DbProviderFactory GetFactory()
        {
            return IBM.Data.DB2.Core.DB2Factory.Instance;
        }
    }
}
