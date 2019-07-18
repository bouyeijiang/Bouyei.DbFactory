/*-------------------------------------------------------------
 *   auth: bouyei
 *   date: 2016/7/12 9:53:15
 *contact: 453840293@qq.com
 *profile: www.openthinking.cn
 *    Ltd: Microsoft
 *   guid: 3cd366b1-356b-4a36-bf5c-aa0decc4bdec
---------------------------------------------------------------*/
using System;
using System.Data.OracleClient;
using System.Data;
using System.Data.Common;

namespace Bouyei.DbFactoryCore.DbAdoProvider
{
    public class DbCommonBuilder:DbAdapterProvider
    {
        protected DbProviderFactory dbProviderFactory  = null;

        protected DbConnection dbConn = null;
        protected DbDataAdapter dbDataAdapter = null;
        protected DbCommand dbCommand = null;
        protected DbTransaction dbTransaction = null;
        protected DbCommonBulkCopy dbBulkCopy = null;
        protected DbCommandBuilder dbCommandBuilder = null;

        protected string ConnectionString { get; private set; }

  
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="IsSingleton"></param>
        protected DbCommonBuilder(DbType dbType):base(dbType)
        {
            dbProviderFactory =  GetAdapterFactory();
          
            if (dbProviderFactory == null)
                throw new Exception("不提供支持该" + dbType.ToString() + "类型的实例");
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

        protected DbDataAdapter CreateAdapter()
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

        protected DbCommand CreateCommand(DbConnection dbConn, Parameter dbParameter, DbTransaction dbTrans = null)
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
                    dbCommand.Parameters.Add((DbParameter)param);
                }
            }

            return dbCommand;
        }

        protected DbTransaction BeginTransaction(DbConnection dbConn, IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            dbTransaction = dbConn.BeginTransaction(isolationLevel);
            return dbTransaction;
        }
 
        protected DbCommonBulkCopy CreateBulkCopy(string ConnectionString,BulkParameter parameter)
        {
            if (dbBulkCopy != null) dbBulkCopy.Dispose();

            if (parameter.IsTransaction)
                dbBulkCopy = new DbCommonBulkCopy(DbType, ConnectionString, CreateConnection(ConnectionString),
                    dbBulkCopyOption: (BulkCopyOptions)parameter.IsolationLevel, isTransaction: parameter.IsTransaction);
            else
                dbBulkCopy = new DbCommonBulkCopy(DbType, ConnectionString);

            return dbBulkCopy;
        }

        protected DbCommonBulkCopy CreateBulkCopy<T>(string ConnectionString, CopyParameter<T> parameter)
        {
            if (dbBulkCopy != null) dbBulkCopy.Dispose();

            if (parameter.IsTransaction)
                dbBulkCopy = new DbCommonBulkCopy(DbType, ConnectionString, CreateConnection(ConnectionString),
                    dbBulkCopyOption: (BulkCopyOptions)parameter.IsolationLevel, isTransaction: parameter.IsTransaction);
            else
                dbBulkCopy = new DbCommonBulkCopy(DbType, ConnectionString);

            return dbBulkCopy;
        }
    }
}
