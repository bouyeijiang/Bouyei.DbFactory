using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace Bouyei.DbFactoryCore.DbAdoProvider
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
            dbProviderFactory = GetProviderFactory();
        }

        public BaseFactory(FactoryType factoryType,int executeTimeout,
            string connectionstring):this(factoryType,executeTimeout)
        {
            this.ConnectionString = connectionstring;
            dbProviderFactory = GetProviderFactory();
        }

        public BaseFactory(FactoryType factoryType,int executeTimeout,
            IDbConnection dbConnection,IDbTransaction dbTransaction)
            :this(factoryType,executeTimeout)
        {
            this.dbConnection =  dbConnection;
            this.dbTransaction = dbTransaction;
            dbProviderFactory = GetProviderFactory();
        }

        protected virtual DbProviderFactory GetProviderFactory()
        {
            switch (DbType)
            {
                case FactoryType.SqlServer:
                    return SqlServer.SqlFactory.GetFactory();
                case FactoryType.Oracle:
                    return  Oracle.OracleFactory.GetFactory();
                case FactoryType.MySql:
                    return  Mysql.MysqlFactory.GetFactory();
                case FactoryType.SQLite:
                    return Sqlite.SqliteFactory.GetFactory();
                case FactoryType.PostgreSQL:
                    return  Postgresql.NpgFactory.GetFactory();
                case FactoryType.DB2:
                    return  DB2.Db2Factory.GetFactory();
                default: throw new Exception("not support type" + DbType);
            }
        }
               
        public virtual void Dispose()
        {
           
        }
    }
}
