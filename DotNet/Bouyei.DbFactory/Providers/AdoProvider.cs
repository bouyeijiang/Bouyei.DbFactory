using System;
using System.Data;

namespace Bouyei.DbFactory
{
    using DbAdoProvider;

    public class AdoProvider : DbProvider,IAdoProvider
    {
        public AdoProvider(string ConnectionString,
            DbType ProviderType = DbType.SqlServer,
            bool IsSingleton = false)
            : base(ConnectionString, ProviderType, IsSingleton)
        {
        }

        public static IAdoProvider CreateProvider(string ConnectionString,
            DbType dbType=DbType.SqlServer, 
            bool IsSingleton = false)
        {
            return new AdoProvider(ConnectionString, dbType, IsSingleton);
        }

        public static IAdoProvider CreateProvider(
            ConnectionConfig connectionConfig)
        {
            return new AdoProvider(connectionConfig.ToString(),
                connectionConfig.DbType);
        }

        public static IAdoProvider Clone(IAdoProvider adoProvider)
        {
            return new AdoProvider(adoProvider.DbConnectionString, adoProvider.DbType);
        }
    }
}
