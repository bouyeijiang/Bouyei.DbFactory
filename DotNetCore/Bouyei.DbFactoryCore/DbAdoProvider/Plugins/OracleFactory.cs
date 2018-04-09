using System;
using System.Data.Common;

namespace Bouyei.DbFactoryCore.DbAdoProvider.Plugins
{
    internal class OracleFactory
    {
        public DbProviderFactory GetFactory()
        {
            return System.Data.OracleClient.OracleClientFactory.Instance;
        }
    }
}
