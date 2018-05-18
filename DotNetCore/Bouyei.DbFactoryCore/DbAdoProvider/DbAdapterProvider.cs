using System;
using System.Data.Common;

namespace Bouyei.DbFactoryCore.DbAdoProvider
{
    using Factories;

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
                    return new SqlFactory().GetFactory();
                case ProviderType.MySql:
                    return new MysqlFactory().GetFactory();
                case ProviderType.SQLite:
                    return new SqliteFactory().GetFactory();
                case ProviderType.PostgreSQL:
                    return new NpgFactory().GetFactory();
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
