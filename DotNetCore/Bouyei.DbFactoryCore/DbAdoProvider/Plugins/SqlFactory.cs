using System;
using System.Data.Common;

namespace Bouyei.DbFactoryCore.DbAdoProvider.Plugins
{
   internal class SqlFactory
    {
        public DbProviderFactory GetFactory()
        {
            return System.Data.SqlClient.SqlClientFactory.Instance;
        }
    }
}
