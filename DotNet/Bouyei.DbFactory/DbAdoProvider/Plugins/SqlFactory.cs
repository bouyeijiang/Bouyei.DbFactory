using System;
using System.Data.Common;

namespace Bouyei.DbFactory.DbAdoProvider.Plugins
{
   internal class SqlFactory
    {
        public DbProviderFactory GetFactory()
        {
            return System.Data.SqlClient.SqlClientFactory.Instance;
        }
    }
}
