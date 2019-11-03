using System;
using System.Data;

namespace Bouyei.DbFactory
{
    using DbAdoProvider;

    public class AdoProvider : DbProvider,IAdoProvider
    {
        public AdoProvider(string ConnectionString,
            FactoryType DbType = FactoryType.SqlServer)
            : base(ConnectionString, DbType)
        {
        }

        public static IAdoProvider CreateProvider(string ConnectionString,
            FactoryType dbType=FactoryType.SqlServer)
        {
            return new AdoProvider(ConnectionString, dbType);
        }

        public static IAdoProvider CreateProvider(
            ConnectionConfig connectionConfig)
        {
            return new AdoProvider(connectionConfig.ToString(),
                connectionConfig.DbType);
        }

        public static IAdoProvider Clone(IAdoProvider adoProvider)
        {
            return new AdoProvider(adoProvider.ConnectionString, adoProvider.DbType);
        }
    }
}
