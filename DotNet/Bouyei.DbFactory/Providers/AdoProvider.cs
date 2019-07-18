using System;
using System.Data;

namespace Bouyei.DbFactory
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
            DbType dbType=DbType.SqlServer)
        {
            return new AdoProvider(ConnectionString, dbType);
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
