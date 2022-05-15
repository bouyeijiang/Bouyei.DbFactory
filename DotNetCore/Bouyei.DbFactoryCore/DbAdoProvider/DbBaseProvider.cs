using System;
using System.Data;
using System.Data.Common;
using OracleDataAccess = Oracle.ManagedDataAccess.Client;

namespace Bouyei.DbFactoryCore.DbAdoProvider
{
    public class DbBaseProvider:BaseFactory
    {
        protected DbConnection dbConn = null;
        protected DbDataAdapter dbDataAdapter = null;
        protected DbCommand dbCommand = null;
        protected DbTransaction dbTransaction = null;
        protected DbCommandBuilder dbCommandBuilder = null;

        public override void Dispose()
        {
            if (this.dbConn != null) this.dbConn.Dispose();
            if (this.dbDataAdapter != null) this.dbDataAdapter.Dispose();
            if (this.dbCommand != null) this.dbCommand.Dispose();
            if (this.dbCommandBuilder != null) dbCommandBuilder.Dispose();
            if (this.dbTransaction != null) dbTransaction.Dispose();
        }

        public DbBaseProvider(FactoryType dbType)
            :base(dbType,1800)
        {
           
        }

        public DbBaseProvider(FactoryType dbType,string ConnectionString)
        : base(dbType, 1800,ConnectionString)
        {
            
        }

        public DbBaseProvider(FactoryType dbType, int ExecuteTimeout, string ConnectionString)
      : base(dbType, ExecuteTimeout, ConnectionString)
        {
            
        }

        protected DbConnection CreateConnection(string ConnectionString)
        {
            if (dbConn != null) dbConn.Dispose();
            dbConn = dbProviderFactory.CreateConnection();

            if (dbConn.ConnectionString != ConnectionString)
            {
                if (dbConn.State != ConnectionState.Closed) dbConn.Close();
                dbConn.ConnectionString = (this.ConnectionString = ConnectionString);
            }

            if (dbConn.State != ConnectionState.Open)
                dbConn.Open();

            return dbConn;
        }

        protected DbDataAdapter CreateDataAdapter()
        {
            if (dbDataAdapter != null) dbDataAdapter.Dispose();
            dbDataAdapter = dbProviderFactory.CreateDataAdapter();

            return dbDataAdapter;
        }

        protected DbCommandBuilder CreateCommandBuilder()
        {
            if (dbCommandBuilder != null) dbCommandBuilder.Dispose();
            dbCommandBuilder = dbProviderFactory.CreateCommandBuilder();

            return dbCommandBuilder;
        }

        protected DbCommand CreateCommand(DbConnection dbConn, Parameter dbParameter,
            DbTransaction dbTrans = null)
        {
            if (dbCommand != null) dbCommand.Dispose();
            dbCommand = dbProviderFactory.CreateCommand();

            dbCommand.Connection = dbConn;

            if (dbTrans != null) dbCommand.Transaction = dbTrans;

            if (dbParameter == null) return dbCommand;

            if (dbParameter.IsStoredProcedure)
                dbCommand.CommandType = CommandType.StoredProcedure;

            dbCommand.CommandText = dbParameter.CommandText;
            dbCommand.CommandTimeout = dbParameter.ExecuteTimeout;

            if (dbParameter.Columns != null)
            {
                foreach (CmdParameter param in dbParameter.Columns)
                {
                    dbCommand.Parameters.Add(CreateParameter(param));
                }
            }

            return dbCommand;
        }

        protected DbTransaction BeginTransaction(DbConnection dbConn, IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            dbTransaction = dbConn.BeginTransaction(isolationLevel);
            return dbTransaction;
        }

        protected DbParameter CreateParameter(CmdParameter parameter)
        {
            DbParameter dbParam = null;

            switch (FactoryType)
            {
                case FactoryType.PostgreSQL:
                    dbParam = Postgresql.NpgFactory.GetParameter(parameter);
                    break;
                case FactoryType.SQLite:
                    dbParam = Sqlite.SqliteFactory.GetParameter(parameter);
                    break;
                case FactoryType.MySql:
                    dbParam = Mysql.MysqlFactory.GetParameter(parameter);
                    break;
                case FactoryType.SqlServer:
                    dbParam = SqlServer.SqlFactory.GetParameter(parameter);
                    break;
                case FactoryType.DB2:
                    dbParam = DB2.Db2Factory.GetParameter(parameter);
                    break;
                case FactoryType.Oracle:
                    dbParam = Oracle.OracleFactory.GetParameter(parameter);
                    break;
                case FactoryType.OleDb:
                    dbParam = OleDb.OleDbFactory.GetParameter(parameter);
                    break;
                case FactoryType.Odbc:
                    dbParam = Odbc.OdbcFactory.GetParameter(parameter);
                    break;
                default:
                    throw new Exception("not supported");
            }
            return dbParam;
        }

    }
}
