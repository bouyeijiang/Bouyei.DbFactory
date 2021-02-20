using System;

namespace Bouyei.DbFactoryCore
{
    using DbEntityProvider;

    public class OrmProvider : EntityProvider,IOrmProvider
    {
        public static IOrmProvider CreateProvider(FactoryType providerType,string DbConnectionString = null)
        {
            return new OrmProvider(providerType,DbConnectionString);
        }

        public static IOrmProvider Clone(IOrmProvider ormProvider)
        {
            return new OrmProvider(ormProvider.DbType, ormProvider.DbConnectionString);
        }

        public OrmProvider(FactoryType providerType,string DbConnectionString = null)
            : base(providerType,DbConnectionString)
        { }
    }
}
