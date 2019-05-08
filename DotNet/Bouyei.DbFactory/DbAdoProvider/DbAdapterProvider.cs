using System;
using System.Data.Common;

namespace Bouyei.DbFactory.DbAdoProvider
{
    using Factories;

    public class DbAdapterProvider
    {
       public DbType DbType { get; set; }

        public DbAdapterProvider(DbType dbType)
        {
            this.DbType = dbType;
        }

        public DbProviderFactory GetAdapterFactory()
        {
            switch (DbType)
            {
                case DbType.SqlServer:
                    return new SqlFactory().GetFactory();
                case DbType.Oracle:
                    return new OracleFactory().GetFactory();
                case DbType.MySql:
                    return new MysqlFactory().GetFactory();
                case DbType.SQLite:
                    return new SqliteFactory().GetFactory();
                case DbType.PostgreSQL:
                    return new NpgFactory().GetFactory();
                case DbType.DB2:
                    return new Db2Factory().GetFactory();
                default: return null;
            }
        }

        public string GetAdapterName(DbType dbType)
        {
            string invariantName = string.Empty;
            switch (dbType)
            {
                case DbType.DB2: invariantName = "IBM.Data.DB2"; break;
                case DbType.Oracle: invariantName = "Oracle.DataAccess"; break;
                //case DbType.MsOracle:invariantName = "System.Data.OracleClient";break;
                case DbType.MySql: invariantName = "MySql.Data.MySqlClient"; break;
                case DbType.SQLite: invariantName = "System.Data.SQLite"; break;
                case DbType.OleDb: invariantName = "System.Data.OleDb"; break;
                case DbType.Odbc: invariantName = "System.Data.Odbc"; break;
                case DbType.PostgreSQL:invariantName = "Npgsql"; break;
                case DbType.SqlServer: invariantName = "System.Data.SqlClient";break;
                default: break;
            }
            return invariantName;
        }
    }
}
