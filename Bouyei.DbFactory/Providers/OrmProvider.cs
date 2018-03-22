using System;

namespace Bouyei.DbFactory
{
    using DbEntityProvider;

    public class OrmProvider : EntityProvider,IOrmProvider
    {
        public static IOrmProvider CreateProvider(string DbConnectionString = null)
        {
            return new OrmProvider(DbConnectionString);
        }

        public static IOrmProvider Clone(IOrmProvider ormProvider)
        {
            return new OrmProvider(ormProvider.DbConnectionString);
        }

        public OrmProvider(string DbConnectionString = null)
            : base(DbConnectionString)
        { }
    }
}
