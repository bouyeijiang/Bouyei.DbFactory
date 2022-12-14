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

        public DbBaseProvider(FactoryType factoryType)
            :base(factoryType,1800)
        {
           
        }

        public DbBaseProvider(FactoryType factoryType,string ConnectionString)
        : base(factoryType, 1800,ConnectionString)
        {
            
        }

        public DbBaseProvider(FactoryType factoryType, int ExecuteTimeout, string ConnectionString)
      : base(factoryType, ExecuteTimeout, ConnectionString)
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

            if (dbParameter.dbProviderParameters != null)
            {
                foreach (CmdParameter param in dbParameter.dbProviderParameters)
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

        protected DbParameter CreateParameter(CmdParameter dbProviderParameter)
        {
            switch (DbType)
            {
                case FactoryType.SqlServer:
                    return new System.Data.SqlClient.SqlParameter()
                    {
                        DbType = dbProviderParameter.DbType,
                        ParameterName = dbProviderParameter.ParameterName,
                        Value = dbProviderParameter.Value,
                        Size = dbProviderParameter.Size,
                        Direction = dbProviderParameter.Direction,
                        SourceColumn = dbProviderParameter.SourceColumn,
                        SourceVersion = dbProviderParameter.SourceVersion,
                        SourceColumnNullMapping = dbProviderParameter.SourceColumnNullMapping
                    };
                case FactoryType.DB2:
                    return new IBM.Data.DB2.Core.DB2Parameter()
                    {
                        DbType = dbProviderParameter.DbType,
                        ParameterName = dbProviderParameter.ParameterName,
                        Value = dbProviderParameter.Value,
                        Size = dbProviderParameter.Size,
                        Direction = dbProviderParameter.Direction,
                        SourceColumn = dbProviderParameter.SourceColumn,
                        SourceVersion = dbProviderParameter.SourceVersion,
                        SourceColumnNullMapping = dbProviderParameter.SourceColumnNullMapping
                    };
                case FactoryType.Oracle:
                    return new OracleDataAccess.OracleParameter()
                    {
                        DbType = dbProviderParameter.DbType,
                        ParameterName = dbProviderParameter.ParameterName,
                        Value = dbProviderParameter.Value,
                        Size = dbProviderParameter.Size,
                        Direction = dbProviderParameter.Direction,
                        SourceColumn = dbProviderParameter.SourceColumn,
                        SourceVersion = dbProviderParameter.SourceVersion,
                        SourceColumnNullMapping = dbProviderParameter.SourceColumnNullMapping
                    };
                case FactoryType.MySql:
                    return new MySql.Data.MySqlClient.MySqlParameter()
                    {
                        DbType = dbProviderParameter.DbType,
                        ParameterName = dbProviderParameter.ParameterName,
                        Value = dbProviderParameter.Value,
                        Size = dbProviderParameter.Size,
                        Direction = dbProviderParameter.Direction,
                        SourceColumn = dbProviderParameter.SourceColumn,
                        SourceVersion = dbProviderParameter.SourceVersion,
                        SourceColumnNullMapping = dbProviderParameter.SourceColumnNullMapping
                    };
                default:
                    return dbProviderParameter;
            }
        }
    }
}
