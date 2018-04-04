using System;
using System.Data.Common;

namespace Bouyei.DbFactory.DbAdoProvider
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
                    return Oracle.DataAccess.Client.OracleClientFactory.Instance;
                case ProviderType.MySql:
                    return MySql.Data.MySqlClient.MySqlClientFactory.Instance;
                case ProviderType.SQLite:
                    return System.Data.SQLite.SQLiteFactory.Instance;
                case ProviderType.PostgreSQL:
                    return Npgsql.NpgsqlFactory.Instance;
                case ProviderType.DB2:
                    return IBM.Data.DB2.DB2Factory.Instance;
                default: return null;
            }
        }

        public string GetAdapterName(ProviderType providerType)
        {
            string invariantName = string.Empty;
            switch (providerType)
            {
                case ProviderType.DB2: invariantName = "IBM.Data.DB2"; break;
                case ProviderType.MsOracle: invariantName = "System.Data.OracleClient"; break;
                case ProviderType.Oracle: invariantName = "Oracle.DataAccess"; break;
                case ProviderType.MySql: invariantName = "MySql.Data.MySqlClient"; break;
                case ProviderType.SQLite: invariantName = "System.Data.SQLite"; break;
                case ProviderType.OleDb: invariantName = "System.Data.OleDb"; break;
                case ProviderType.Odbc: invariantName = "System.Data.Odbc"; break;
                case ProviderType.PostgreSQL:invariantName = "Npgsql"; break;
                case ProviderType.SqlServer: invariantName = "System.Data.SqlClient";break;
                default: break;
            }
            return invariantName;
        }
    }
}
