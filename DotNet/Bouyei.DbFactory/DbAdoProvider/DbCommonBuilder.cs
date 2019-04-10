/*-------------------------------------------------------------
 *   auth: bouyei
 *   date: 2016/7/12 9:53:15
 *contact: 453840293@qq.com
 *profile: www.openthinking.cn
 *    Ltd: Microsoft
 *   guid: 3cd366b1-356b-4a36-bf5c-aa0decc4bdec
---------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

using System.Reflection;

namespace Bouyei.DbFactory.DbAdoProvider
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
        protected DbType DbProviderType { get; private set; }

        protected bool IsSingleton { get; private set; }

        protected string ConnectionString { get; private set; }

        //默认预设的工厂提供动态实例，可以直接在app.config配置
        private static Dictionary<DbType, AssemblyFactoryInfo> AssemblyCache
            = new Dictionary<DbType, AssemblyFactoryInfo>();

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="dbProviderType"></param>
        /// <param name="IsSingleton"></param>
        protected DbCommonBuilder(DbType dbProviderType,
             bool IsSingleton):
            base(dbProviderType)
        {
            this.IsSingleton = IsSingleton;
            this.DbProviderType = dbProviderType;

            string invariantName = GetAdapterName(dbProviderType);

            dbProviderFactory = GetDbFactory(invariantName);

            if (dbProviderFactory == null)
                throw new Exception("不提供支持该" + dbProviderType.ToString() + "类型的实例");
        }

        protected DbConnection CreateConnection(string ConnectionString)
        {
            if (IsSingleton)
            {
                if (dbConn == null)
                    dbConn = dbProviderFactory.CreateConnection();
            }
            else
            {
                if (dbConn != null) dbConn.Dispose();
                dbConn = dbProviderFactory.CreateConnection();
            }
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
            if (IsSingleton)
            {
                if (dbDataAdapter == null)
                    dbDataAdapter = dbProviderFactory.CreateDataAdapter();
            }
            else
            {
                if (dbDataAdapter != null) dbDataAdapter.Dispose();
                dbDataAdapter = dbProviderFactory.CreateDataAdapter();
            }
            return dbDataAdapter;
        }

        protected DbCommandBuilder CreateCommandBuilder()
        {
            if (IsSingleton)
            {
                if (dbCommandBuilder == null)
                    dbCommandBuilder = dbProviderFactory.CreateCommandBuilder();
            }
            else
            {
                if (dbCommandBuilder != null) dbCommandBuilder.Dispose();
                dbCommandBuilder = dbProviderFactory.CreateCommandBuilder();
            }
            return dbCommandBuilder;
        }

        protected DbCommand CreateCommand(DbConnection dbConn, Parameter dbParameter, DbTransaction dbTrans = null)
        {
            if (IsSingleton)
            {
                if (dbCommand == null)
                    dbCommand = dbProviderFactory.CreateCommand();
            }
            else
            {
                if (dbCommand != null) dbCommand.Dispose();
                dbCommand = dbProviderFactory.CreateCommand();
            }

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
            switch (DbProviderType)
            {
                case DbType.SqlServer:
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
                //case ProviderType.DB2:
                //    return new IBM.Data.DB2.DB2Parameter()
                //    {
                //        DbType = dbProviderParameter.DbType,
                //        ParameterName = dbProviderParameter.ParameterName,
                //        Value = dbProviderParameter.Value,
                //        Size = dbProviderParameter.Size,
                //        Direction = dbProviderParameter.Direction,
                //        SourceColumn = dbProviderParameter.SourceColumn,
                //        SourceVersion = dbProviderParameter.SourceVersion,
                //        SourceColumnNullMapping = dbProviderParameter.SourceColumnNullMapping
                //    };
                case DbType.Oracle:
                    return new Oracle.DataAccess.Client.OracleParameter()
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
                case DbType.MySql:
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
         
        protected DbCommonBulkCopy CreateBulkCopy(string ConnectionString,BulkParameter parameter)
        {
            if (dbBulkCopy != null) dbBulkCopy.Dispose();

            if (parameter.IsTransaction)
                dbBulkCopy = new DbCommonBulkCopy(DbProviderType, ConnectionString, CreateConnection(ConnectionString),
                    dbBulkCopyOption: (BulkCopyOptions)parameter.IsolationLevel, isTransaction: parameter.IsTransaction);
            else
                dbBulkCopy = new DbCommonBulkCopy(DbProviderType, ConnectionString);

            return dbBulkCopy;
        }

        private DbProviderFactory GetDbFactory(string invariantName)
        {
            if (ExistsDbProviderFactories(invariantName))
                return DbProviderFactories.GetFactory(invariantName);
            else
            {
                return GetAdapterFactory();

                //AssemblyFactoryInfo assemInfo = null;
                //AssemblyCache.TryGetValue(DbProviderType, out assemInfo);
                //if (assemInfo == null)
                //{
                //    assemInfo = GetDynamicDllProviderInfo(invariantName);
                //    assemInfo.FactoryName = GetAdapterName(DbProviderType);
                //    AssemblyCache.Add(DbProviderType, assemInfo);
                //}

                //Type type = Type.GetType(assemInfo.ToString());
                //if (type == null) return null;

                //FieldInfo field = type.GetField("Instance", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
                //if (field == null || !field.FieldType.IsSubclassOf(typeof(DbProviderFactory))) return null;

                //object obj = field.GetValue(null);
                //if (obj == null) return null;

                //return (DbProviderFactory)obj;
            }
        }

        //private string GetFactoryName(ProviderType providerType)
        //{
        //    string factoryName = "System.Data.SqlClient.SqlClientFactory";

        //    switch (providerType)
        //    {
        //        case ProviderType.DB2:
        //            factoryName = "IBM.Data.DB2.DB2Factory"; break;
        //        case ProviderType.Oracle:
        //            factoryName = "Oracle.DataAccess.Client.OracleClientFactory"; break;
        //        case ProviderType.MySql:
        //            factoryName = "MySql.Data.MySqlClient.MySqlClientFactory"; break;
        //        case ProviderType.SQLite:
        //            factoryName = "Devart.Data.SQLite.SQLiteProviderFactory"; break;
        //        case ProviderType.OleDb:
        //            factoryName = "System.Data.OleDb.Ole"; break;
        //        case ProviderType.Odbc:
        //            factoryName = "System.Data.Odbc.OdbcFactory"; break;
        //        case ProviderType.SqlServer:
        //            factoryName = "System.Data.SqlClient.SqlClientFactory"; break;
        //        case ProviderType.MsOracle:
        //            factoryName = "System.Data.OracleClient.OracleClientFactory"; break;
        //        case ProviderType.PostgreSQL:
        //            factoryName = "Npgsql.NpgsqlFactory";break;
        //        default: break;
        //    }
        //    return factoryName;
        //}

        private bool ExistsDbProviderFactories(string invariantName)
        {
            return DbProviderFactories.GetFactoryClasses().Rows.Contains(invariantName);
        }

        private AssemblyFactoryInfo GetDynamicDllProviderInfo(string invariantName)
        {
            string path = string.Empty;
            try
            {
                path = AppDomain.CurrentDomain.BaseDirectory + invariantName + ".dll";
                Assembly assem = Assembly.LoadFile(path);

                return new AssemblyFactoryInfo(assem.FullName);
            }
            catch (Exception ex)
            {
                throw new Exception("path:" + path + ";" + ex.ToString());
            }
        }
    }
}
