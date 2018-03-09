using System;
using System.Data;

namespace Bouyei.DbFactory
{
    using DbAdoProvider;

    public class AdoProvider : DbProvider,IAdoProvider
    {
        public AdoProvider(string ConnectionString,
            ProviderType ProviderType = ProviderType.SqlServer,
            bool IsSingleton = false)
            : base(ConnectionString, ProviderType, IsSingleton)
        {
        }

        public static IAdoProvider CreateProvider(string ConnectionString,
            ProviderType providerType=ProviderType.SqlServer, 
            bool IsSingleton = false)
        {
            return new AdoProvider(ConnectionString, providerType, IsSingleton);
        }

        public static IAdoProvider CreateProvider(
            ConnectionConfig connectionConfig)
        {
            return new AdoProvider(connectionConfig.ToString(),
                connectionConfig.DbType);
        }

        public static IAdoProvider Clone(IDbProvider dbProvider)
        {
            return new AdoProvider(dbProvider.DbConnectionString, dbProvider.DbType);
        }
    }
}
