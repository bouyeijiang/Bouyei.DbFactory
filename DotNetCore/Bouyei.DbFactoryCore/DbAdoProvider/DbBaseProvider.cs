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

            if (dbParameter.cmdParameters != null)
            {
                foreach (CmdParameter param in dbParameter.cmdParameters)
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

        protected DbParameter CreateParameter(CmdParameter cmdParameter)
        {
            DbParameter dbParam = null;

            switch (FactoryType)
            {
                case FactoryType.PostgreSQL:
                    dbParam = new Npgsql.NpgsqlParameter(cmdParameter.ParameterName, cmdParameter.Value);
                    break;
                case FactoryType.SQLite:
                    dbParam = new System.Data.SQLite.SQLiteParameter(cmdParameter.ParameterName, cmdParameter.Value);
                    break;
                case FactoryType.MySql:
                    dbParam = new MySql.Data.MySqlClient.MySqlParameter(cmdParameter.ParameterName, cmdParameter.Value);
                    break;
                case FactoryType.SqlServer:
                    dbParam = new System.Data.SqlClient.SqlParameter(cmdParameter.ParameterName, cmdParameter.Value);
                    break;
                case FactoryType.DB2:
                    dbParam = new IBM.Data.DB2.Core.DB2Parameter(cmdParameter.ParameterName, cmdParameter.Value);
                    break;
                case FactoryType.Oracle:
                    dbParam = new OracleDataAccess.OracleParameter(cmdParameter.ParameterName, cmdParameter.Value);
                    break;
                default:
                    throw new Exception("not supported");
            }

            dbParam.DbType = cmdParameter.DbType;
            dbParam.Size = cmdParameter.Size;
            dbParam.Direction = cmdParameter.Direction;
            dbParam.SourceColumn = cmdParameter.SourceColumn;
            dbParam.SourceVersion = cmdParameter.SourceVersion;
            dbParam.SourceColumnNullMapping = cmdParameter.SourceColumnNullMapping;

            return dbParam;
        }

    }
}
