using System;
using System.Data;
using System.Data.Common;

namespace Bouyei.DbFactory.DbAdoProvider
{
    public class BaseFactory:IBaseFactory
    {
        public string ConnectionString { get;  set; }

        public int ExecuteTimeout { get;  set; }

        public FactoryType DbType { get; set; }

        protected DbProviderFactory dbProviderFactory { get;private set; }

        private IDbConnection dbConnection = null;
        private IDbTransaction dbTransaction = null;

        public BaseFactory(FactoryType dbType,int executeTimeout)
        {
            this.DbType = dbType;
            this.ExecuteTimeout = executeTimeout;
        }

        public BaseFactory(FactoryType dbType,int executeTimeout,
            string connectionstring):this(dbType,executeTimeout)
        {
            this.ConnectionString = connectionstring;
            dbProviderFactory = GetProviderFactory();
        }

        public BaseFactory(FactoryType dbType,int executeTimeout,
            IDbConnection dbConnection,IDbTransaction dbTransaction)
            :this(dbType,executeTimeout)
        {
            this.dbConnection =  dbConnection;
            this.dbTransaction = dbTransaction;
            dbProviderFactory = GetProviderFactory();
        }

        public virtual DbProviderFactory GetProviderFactory()
        {
            switch (DbType)
            {
                case FactoryType.SqlServer:
                    return SqlServer.SqlFactory.GetFactory();
                case FactoryType.Oracle:
                    return Oracle.OracleFactory.GetFactory();
                case FactoryType.MySql:
                    return Mysql.MysqlFactory.GetFactory();
                case FactoryType.SQLite:
                    return Sqlite.SqliteFactory.GetFactory();
                case FactoryType.PostgreSQL:
                    return  Postgresql.NpgFactory.GetFactory();
                case FactoryType.DB2:
                    return DB2.Db2Factory.GetFactory();
                default:
                    {
                        string invariant = GetFactoryName();
                        if (invariant == string.Empty) throw new Exception("not support type" + DbType);
                        return DbProviderFactories.GetFactory(invariant);
                    }
            }
        }


        public string GetFactoryName()
        {
            string invariantName = string.Empty;
            switch (DbType)
            {
                case FactoryType.DB2: invariantName = "IBM.Data.DB2"; break;
                case FactoryType.Oracle: invariantName = "Oracle.DataAccess"; break;
                case FactoryType.MySql: invariantName = "MySql.Data.MySqlClient"; break;
                case FactoryType.SQLite: invariantName = "System.Data.SQLite"; break;
                case FactoryType.OleDb: invariantName = "System.Data.OleDb"; break;
                case FactoryType.Odbc: invariantName = "System.Data.Odbc"; break;
                case FactoryType.PostgreSQL: invariantName = "Npgsql"; break;
                case FactoryType.SqlServer: invariantName = "System.Data.SqlClient"; break;
                default: break;
            }
            return invariantName;
        }

        public virtual void Dispose()
        {
           
        }
    }
}
