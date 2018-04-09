using System;
using System.Data.Common;

namespace Bouyei.DbFactoryCore.DbAdoProvider.Plugins
{
   internal class NpgFactory
    {
        public DbProviderFactory GetFactory()
        {
            return Npgsql.NpgsqlFactory.Instance;
        }
    }
}
