using System;
using System.Data;

namespace Bouyei.DbFactoryCore
{
    using DbAdoProvider;

    public class AdoProvider : DbProvider,IAdoProvider
    {
        public AdoProvider(string ConnectionString,
            DbType DbType = DbType.SqlServer)
            : base(ConnectionString, DbType)
        {
        }

        public static IAdoProvider CreateProvider(string ConnectionString,
            DbType providerType=DbType.SqlServer)
        {
            return new AdoProvider(ConnectionString, providerType);
        }

        public static IAdoProvider Clone(IAdoProvider adoProvider)
        {
            return new AdoProvider(adoProvider.DbConnectionString, adoProvider.DbType);
        }
    }
}
