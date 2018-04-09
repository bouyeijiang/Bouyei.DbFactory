using System;
using System.Data.Common;

namespace Bouyei.DbFactoryCore.DbAdoProvider.Plugins
{
   internal class SqliteFactory
    {
        public DbProviderFactory GetFactory()
        {
            return Microsoft.Data.Sqlite.SqliteFactory.Instance;
        }
    }
}
