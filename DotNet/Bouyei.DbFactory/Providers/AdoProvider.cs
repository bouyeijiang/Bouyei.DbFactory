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
            FactoryType DbType=FactoryType.PostgreSQL)
        {
            return new AdoProvider(ConnectionString, DbType);
        }

        public static IAdoProvider CreateProvider(
            ConnectionConfig connectionConfig)
        {
            return new AdoProvider(connectionConfig.ToString(),
                connectionConfig.DbType);
        }

        public static IAdoProvider Clone(IAdoProvider adoProvider)
        {
            return new AdoProvider(adoProvider.ConnectionString, adoProvider.FactoryType);
        }
    }
}
