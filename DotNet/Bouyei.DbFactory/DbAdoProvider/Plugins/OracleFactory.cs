using System;
using System.Data.Common;

namespace Bouyei.DbFactory.DbAdoProvider.Plugins
{
    internal class OracleFactory
    {
        public DbProviderFactory GetFactory()
        {
            return Oracle.DataAccess.Client.OracleClientFactory.Instance;
        }
    }
}
