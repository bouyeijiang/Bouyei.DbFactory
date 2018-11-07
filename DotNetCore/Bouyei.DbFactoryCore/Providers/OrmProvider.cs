using System;

namespace Bouyei.DbFactoryCore
{
    using DbEntityProvider;

    public class OrmProvider : EntityProvider,IOrmProvider
    {
        public static IOrmProvider CreateProvider(DbType providerType,string DbConnectionString = null)
        {
            return new OrmProvider(providerType,DbConnectionString);
        }

        public static IOrmProvider Clone(IOrmProvider ormProvider)
        {
            return new OrmProvider(ormProvider.ProviderType, ormProvider.DbConnectionString);
        }

        public OrmProvider(DbType providerType,string DbConnectionString = null)
            : base(providerType,DbConnectionString)
        { }
    }
}
