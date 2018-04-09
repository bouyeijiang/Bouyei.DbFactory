using System;
using System.Data.Common;

namespace Bouyei.DbFactory.DbAdoProvider.Plugins
{
   internal class NpgFactory
    {
        public DbProviderFactory GetFactory()
        {
            return Npgsql.NpgsqlFactory.Instance;
        }
    }
}
