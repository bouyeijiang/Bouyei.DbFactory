using System;

namespace Bouyei.DbFactory
{
    using DbEntityProvider;

    public class OrmProvider : EntityProvider,IOrmProvider
    {
        public static new OrmProvider CreateProvider(string DbConnection = null)
        {
            return new OrmProvider(DbConnection);
        }

        public static OrmProvider Clone(IOrmProvider ormProvider)
        {
            return new OrmProvider(ormProvider.DbConnectionString);
        }

        public OrmProvider(string DbConnection = null)
            : base(DbConnection)
        { }
    }
}
