using System;
using System.Data.Common;

namespace Bouyei.DbFactory.DbAdoProvider.Plugins
{
   internal class SqliteFactory
    {
        public DbProviderFactory GetFactory()
        {
            return System.Data.SQLite.SQLiteFactory.Instance;
        }
    }
}
