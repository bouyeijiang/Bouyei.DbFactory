using System;
using System.Data.Common;

namespace Bouyei.DbFactoryCore.DbAdoProvider
{
    public class DbAdapterProvider
    {
        ProviderType providerType;
        public DbAdapterProvider(ProviderType providerType)
        {
            this.providerType = providerType;
        }

        public DbProviderFactory GetAdapterFactory()
        {
            switch (providerType)
            {
                case ProviderType.SqlServer:
                    return System.Data.SqlClient.SqlClientFactory.Instance;
                case ProviderType.Oracle:
                    return System.Data.OracleClient.OracleClientFactory.Instance;
                case ProviderType.MySql:
                    return MySql.Data.MySqlClient.MySqlClientFactory.Instance;
                case ProviderType.SQLite:
                    return Microsoft.Data.Sqlite.SqliteFactory.Instance;
                case ProviderType.PostgreSQL:
                    return Npgsql.NpgsqlFactory.Instance;
                case ProviderType.DB2:
                    return IBM.Data.DB2.Core.DB2Factory.Instance;
                default: return null;
            }
        }

        public string GetAdapterName(ProviderType providerType)
        {
            string invariantName = string.Empty;
            switch (providerType)
            {
                case ProviderType.DB2: invariantName = "IBM.Data.DB2"; break;
                case ProviderType.Oracle: invariantName = "Oracle.DataAccess"; break;
                case ProviderType.MySql: invariantName = "MySql.Data.MySqlClient"; break;
                case ProviderType.SQLite: invariantName = "System.Data.SQLite"; break;
                case ProviderType.PostgreSQL:invariantName = "Npgsql"; break;
                case ProviderType.SqlServer: invariantName = "System.Data.SqlClient";break;
                default: break;
            }
            return invariantName;
        }
    }
}
