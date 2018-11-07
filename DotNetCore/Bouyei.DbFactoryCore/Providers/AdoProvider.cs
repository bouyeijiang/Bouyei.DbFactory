using System;
using System.Data;

namespace Bouyei.DbFactoryCore
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
            DbType providerType=DbType.SqlServer, 
            bool IsSingleton = false)
        {
            return new AdoProvider(ConnectionString, providerType, IsSingleton);
        }

        public static IAdoProvider Clone(IAdoProvider adoProvider)
        {
            return new AdoProvider(adoProvider.DbConnectionString, adoProvider.DbType);
        }
    }
}
