using System;
using System.Data;

namespace Bouyei.DbFactoryCore
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
            FactoryType providerType=FactoryType.SqlServer)
        {
            return new AdoProvider(ConnectionString, providerType);
        }

        public static IAdoProvider Clone(IAdoProvider adoProvider)
        {
            return new AdoProvider(adoProvider.ConnectionString, adoProvider.DbType);
        }
    }
}
