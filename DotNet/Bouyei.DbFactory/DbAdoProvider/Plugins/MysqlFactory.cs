using System;
using System.Data.Common;

namespace Bouyei.DbFactory.DbAdoProvider.Plugins
{
   internal class MysqlFactory
    {
        public DbProviderFactory GetFactory()
        {
            return MySql.Data.MySqlClient.MySqlClientFactory.Instance;
        }
    }
}
