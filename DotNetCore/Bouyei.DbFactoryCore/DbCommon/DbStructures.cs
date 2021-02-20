/*-------------------------------------------------------------
 *project:Bouyei.DbFactoryCore.DbCommon
 *   auth: bouyei
 *   date: 2017/11/29 16:40:32
 *contact: 453840293@qq.com
 *profile: www.openthinking.cn
---------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Bouyei.DbFactoryCore
{
    public delegate void BulkCopiedArgs(long rows);

    public delegate void SyncProgressArgs(SyncProgressInfo info);

    public delegate void SyncStateArgs(SyncStateInfo info);

    public class CmdParameter : DbParameter
    {
        public override System.Data.DbType DbType { get; set; }

        public override string ParameterName { get; set; }

        public override int Size { get; set; }

        public override object Value { get; set; }

        public override ParameterDirection Direction { get; set; }

        public override string SourceColumn { get; set; }

        public override DataRowVersion SourceVersion { get; set; }

        public override bool SourceColumnNullMapping { get; set; }

        public override bool IsNullable { get; set; }

        public override void ResetDbType()
        {
            throw new NotImplementedException();
        }
    }

    public class Parameter:BaseParameter
    {
        public Parameter(int ExecuteTimeout)
            :base(ExecuteTimeout)
        { }

        public Parameter(params CmdParameter[] dbProviderParameters)
        {
            this.dbProviderParameters = dbProviderParameters;
        }

        public Parameter(string CommandText,
            int ExectueTimeout = 1800,
            CmdParameter[] dbProviderParameters = null)
            :base(ExectueTimeout)
        {
            this.CommandText = CommandText;
            this.dbProviderParameters = dbProviderParameters;
        }
        public Parameter(string format,params object[] args)
            :base()
        {
            this.CommandText = string.Format(format, args);
        }

        /// <summary>
        /// 执行脚本的语句
        /// </summary>
        public string CommandText { get; set; }
        /// <summary>
        /// 脚本是否为存储过程
        /// </summary>
        public bool IsStoredProcedure { get; set; }

        /// <summary>
        /// 指定脚本的传入参数
        /// </summary>
        public CmdParameter[] dbProviderParameters { get; set; }
    }
    
    public class BulkParameter : BaseParameter
    {
        public BulkParameter()
            : base()
        { }

        public BulkParameter(DataTable dataSource,
            int BatchSize = 10240,
            int ExecuteTimeout = 1800)
            : base(ExecuteTimeout)
        {
            this.DataSource = dataSource;
            this.TableName = dataSource.TableName;
            this.batchSize = BatchSize;
        }

        public BulkParameter(string tableName, IDataReader iDataReader,
           int batchSize = 10240,
           int executeTimeout = 1800,
           bool isTransaction = false)
            : base(executeTimeout)
        {
            this.IDataReader = iDataReader;
            this.TableName = tableName;
            this.batchSize = batchSize;
        }

        public string TableName { get; private set; }
        /// <summary>
        /// 如果使用DataTable该数据集批量写入，必需设置TableName
        /// </summary>
        public DataTable DataSource { get; set; }

        public IDataReader IDataReader { get; set; }

        private int batchSize = 10240;
        /// <summary>
        /// 批量大小,默认10240
        /// </summary>
        public int BatchSize { get { return batchSize; } set { batchSize = value; } }

        public bool IsAutoDispose { get; set; }

        public Action<IDbTransaction, int> TransactionCallback { get; set; }

        public BulkCopiedArgs BulkCopiedHandler { get; set; }
    }
    public class CopyParameter<T> : BaseParameter
    {
        public CopyParameter()
            : base()
        { }

        public CopyParameter(T dataSource,
            int BatchSize = 10240,
            int ExecuteTimeout = 1800)
            : base(ExecuteTimeout)
        {
            this.dataSource = dataSource;
            this.BatchSize = BatchSize;

            if (dataSource is DataTable)
                TableName = (dataSource as DataTable).TableName;
            else if (dataSource is Array)
                this.TableName = ((dataSource as Array).GetValue(0)).GetType().Name;
            else if (dataSource is IDataReader)
                this.TableName = (dataSource as IDataReader).GetSchemaTable().TableName;
            else throw new Exception("not support data type");
        }

        public int BatchSize { get; set; } = 1024;

        public string TableName { get; set; }

        public T dataSource { get; set; }

        public BulkCopiedArgs BulkCopiedHandler { get; set; }
    }
    [Serializable]
    public class DbResult<R, I>
    {
        public R Result { get; set; }

        public I Info { get; set; }

        public DbResult()
        {
        }

        public DbResult(R Result)
        {
            this.Result = Result;
        }

        public DbResult(I Info)
        {
            this.Info = Info;
        }

        public DbResult(R Result, I Info)
        {
            this.Result = Result;
            this.Info = Info;
        }

        public static DbResult<R, I> Create(R Result, I Info)
        {
            return new DbResult<R, I>(Result, Info);
        }
    }
 
    public class BaseParameter
    {
        /// <summary>
        /// 超时默认值,1800s
        /// </summary>
        public int ExecuteTimeout { get; set; }

        /// <summary>
        /// 是否使用事务
        /// </summary>
        public bool IsTransaction { get; private set; }

        public IsolationLevel IsolationLevel { get; private set; }

        public void SetTransaction(IsolationLevel isolationLevel)
        {
            IsTransaction = true;
            IsolationLevel = isolationLevel;
        }

        public BaseParameter(int ExecuteTimeout=1800)
        {
            this.ExecuteTimeout = ExecuteTimeout;
        }
    }

    public class SyncTableSchema
    {
        public string TableName { get; set; }

        public List<SyncColumnName> Columns { get; set; }
    }

    public class SyncFilterSchema
    {
        public string TableName { get; set; }

        public string FilterClause { get; set; }

        public List<string> FilterColumns { get; set; }
    }

    public class SyncColumnName
    {
        public SyncColumnName()
        { }

        public SyncColumnName(string ColumnName)
        {
            this.ColumnName = ColumnName;
        }
        public string ColumnName { get; set; }

        public string DataType { get; set; }

        public int Size { get; set; }

        public bool IsPrimaryKey { get; set; }

        public int IncrementStep { get; set; }

        public int IncrementStart { get; set; }
    }

    public class SyncResultInfo
    {
        public SyncResultInfo() { }
        //
        // 摘要:
        //     获取或设置同步会话开始的日期和时间。
        //
        // 返回结果:
        //     同步会话开始的日期和时间。
        public DateTime SyncStartTime { get; set; }
        //
        // 摘要:
        //     获取或设置同步会话结束的日期和时间。
        //
        // 返回结果:
        //     同步会话结束的日期和时间。
        public DateTime SyncEndTime { get; set; }
        //
        // 摘要:
        //     获取在下载会话期间成功应用的变更的总数。
        //
        // 返回结果:
        //     在下载会话期间成功应用的变更的总数。如果未发生下载会话，则返回 0。
        public int DownloadChangesApplied { get; set; }
        //
        // 摘要:
        //     获取在下载会话期间未应用的变更的总数。
        //
        // 返回结果:
        //     在下载会话期间未应用的变更的总数。
        public int DownloadChangesFailed { get; set; }
        //
        // 摘要:
        //     获取在下载会话期间尝试的变更的总数。
        //
        // 返回结果:
        //     在下载会话期间尝试的变更的总数。
        public int DownloadChangesTotal { get; set; }
        //
        // 摘要:
        //     获取在上载会话期间成功应用的变更的总数。
        //
        // 返回结果:
        //     在上载会话期间成功应用的变更的总数。
        public int UploadChangesApplied { get; set; }
        //
        // 摘要:
        //     获取在上载会话期间应用失败的变更的总数。
        //
        // 返回结果:
        //     在上载会话期间应用失败的变更的总数。
        public int UploadChangesFailed { get; set; }
        //
        // 摘要:
        //     获取在上载会话期间尝试的变更的总数。
        //
        // 返回结果:
        //     在上载会话期间尝试的变更的总数。
        public int UploadChangesTotal { get; set; }
    }

    public class SyncParameter
    {
        public SyncDirectionType Direction { get; set; }
    }

    public class SyncProgressInfo
    {
        public int CompletedValue { get; set; }

        public int TotalValue { get; set; }

        public string SessionStage { get; set; }

        public string SynPosition { get; set; }
    }

    public class SyncStateInfo
    {
        public string OldState { get; set; }

        public string NewState { get; set; }
    }

    [Serializable]
    public class ConnectionConfig
    {
        public FactoryType DbType { get; set; }

        public string DbIp { get; set; }

        public int DbPort { get; set; }

        public string DbName { get; set; }

        public string DbUserName { get; set; }

        public string DbPassword { get; set; }

        public string ConnectionString { get; private set; }

        public override string ToString()
        {
            switch (DbType)
            {
                case FactoryType.SqlServer:
                    {
                        if (DbPort <= 0) DbPort = 1433;
                        ConnectionString = string.Format("Server={0},{1};Database={2};User Id={3};Password={4};",
                            DbIp, DbPort, DbName, DbUserName, DbPassword);
                    }
                    break;
                case FactoryType.Oracle:
                    //case DbType.MsOracle:
                    {
                        if (DbPort <= 0) DbPort = 1521;

                        if (string.IsNullOrEmpty(DbName))
                            DbName = "ORCL";
                        ConnectionString = string.Format("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}))(CONNECT_DATA=(SERVICE_NAME={2})));User Id={3};Password={4};",
                             DbIp, DbPort, DbName, DbUserName, DbPassword);
                    }
                    break;
                case FactoryType.DB2:
                    {
                        if (DbPort <= 0) DbPort = 50000;

                        ConnectionString = string.Format("server={0},{1};database={2};uid={3};pwd={4};",
                            DbIp, DbPort, DbName, DbUserName, DbPassword);
                    }
                    break;
                case FactoryType.MySql:
                    {
                        if (DbPort <= 0) DbPort = 3306;
                        ConnectionString = string.Format("Data Source={0};port={1};Database={2};User Id={3};Password={4};pooling=false;CharSet=utf8;",
                          DbIp, DbPort, DbName, DbUserName, DbPassword);
                    }
                    break;
                case FactoryType.SQLite:
                    {
                        ConnectionString = string.Format("Data Source={0};Version=3;", DbIp);
                        if (string.IsNullOrEmpty(DbPassword) == false)
                            ConnectionString += "Password=" + DbPassword;
                    }
                    break;
                case FactoryType.PostgreSQL:
                    ConnectionString = $"Server={DbIp};Port={DbPort};UserId={DbUserName};Password={DbPort};Database={DbName};";
                    break;
                case FactoryType.Odbc:
                    ConnectionString = $"Driver={{{DbName}}};Server={DbIp};Database={DbName}; Uid={DbUserName};Pwd={DbPassword};";
                    break;
                case FactoryType.OleDb:
                    {
                        ConnectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={DbName};";
                        if (string.IsNullOrEmpty(DbUserName) == false)
                            ConnectionString += $"User ID={DbUserName};";
                        if (string.IsNullOrEmpty(DbPassword) == false)
                            ConnectionString += $"Password={DbPassword};";
                    }
                    break;
                default:
                    ConnectionString = "未知连接类型";
                    break;
            }
            return ConnectionString;
        }
    }

}
