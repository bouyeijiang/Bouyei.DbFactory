using System;
using System.Data;

namespace Bouyei.DbFactoryCore
{
    using DbAdoProvider;

    public class AdoProvider : DbProvider,IAdoProvider
    {
        public AdoProvider(string ConnectionString,
            FactoryType DbType = FactoryType.PostgreSQL)
            : base(ConnectionString, DbType)
        {
        }

        public static IAdoProvider CreateProvider(string ConnectionString,
            FactoryType DbType=FactoryType.PostgreSQL)
        {
            return new AdoProvider(ConnectionString, DbType);
        }

        public static IAdoProvider CreateProvider(ConnectionConfig ConnectionString,
          FactoryType DbType = FactoryType.PostgreSQL)
        {
            return new AdoProvider(ConnectionString.ToString(), DbType);
        }

        public static IAdoProvider Clone(IAdoProvider adoProvider)
        {
            return new AdoProvider(adoProvider.ConnectionString, adoProvider.DbType);
        }
    }
}
