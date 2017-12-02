/*-------------------------------------------------------------
 *project:Bouyei.DbFactory.DbCommon
 *   auth: bouyei
 *   date: 2017/11/29 16:40:32
 *contact: 453840293@qq.com
 *profile: www.openthinking.cn
---------------------------------------------------------------*/
using System;
using System.Data;
using System.Data.Common;

namespace Bouyei.DbFactory
{
    public delegate void BulkCopiedArgs(long rows);

    public class DbProviderParameter : DbParameter
    {
        public override DbType DbType { get; set; }

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

    public class DbExecuteParameter:BaseParameter
    {
        public DbExecuteParameter(int ExecuteTimeout)
            :base(ExecuteTimeout)
        { }

        public DbExecuteParameter(params DbProviderParameter[] dbProviderParameters)
        {
            this.dbProviderParameters = dbProviderParameters;
        }

        public DbExecuteParameter(string CommandText,
            int ExectueTimeout = 1800,
            DbProviderParameter[] dbProviderParameters = null)
            :base(ExectueTimeout)
        {
            this.CommandText = CommandText;
            this.dbProviderParameters = dbProviderParameters;
        }
        /// <summary>
        /// 查询映射对象名忽略大小写
        /// </summary>
        public bool IgnoreCase { get; set; }
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
        public DbProviderParameter[] dbProviderParameters { get; set; }
    }

    public class DbExecuteBulkParameter : BaseParameter
    {
        public DbExecuteBulkParameter()
            : base()
        { }

        public DbExecuteBulkParameter(DataTable dataSource,
            int BatchSize = 10240,
            int ExecuteTimeout = 1800,
            bool IsTransaction = false)
            : base(ExecuteTimeout)
        {
            this.DataSource = dataSource;
            this.TableName = dataSource.TableName;
            this.batchSize = BatchSize;
            this.IsTransaction = IsTransaction;
        }

        public DbExecuteBulkParameter(string tableName, IDataReader iDataReader,
           int batchSize = 10240,
           int executeTimeout = 1800,
           bool isTransaction = false)
            : base(executeTimeout)
        {
            this.IDataReader = iDataReader;
            this.TableName = tableName;
            this.batchSize = batchSize;
            this.IsTransaction = isTransaction;
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

        /// <summary>
        /// 是否启用事务
        /// </summary>
        public bool IsTransaction { get; set; }

        public bool IsAutoDispose { get; set; }

        public Action<IDbTransaction, int> TransactionCallback { get; set; }

        public BulkCopiedArgs BulkCopiedHandler { get; set; }
    }

    [Serializable]
    public class ResultInfo<R, I>
    {
        public R Result { get; set; }

        public I Info { get; set; }

        public ResultInfo()
        {
        }

        public ResultInfo(R Result)
        {
            this.Result = Result;
        }

        public ResultInfo(I Info)
        {
            this.Info = Info;
        }

        public ResultInfo(R Result, I Info)
        {
            this.Result = Result;
            this.Info = Info;
        }

        public static ResultInfo<R, I> Create(R Result, I Info)
        {
            return new ResultInfo<R, I>(Result, Info);
        }
    }

    [Serializable]
    public class ConnectionConfiguration
    {
        public ProviderType DbType { get; set; }

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
                case ProviderType.SqlServer:
                    {
                        if (DbPort <= 0) DbPort = 1433;
                        ConnectionString = string.Format("Server={0},{1};Database={2};User Id={3};Password={4};",
                            DbIp, DbPort, DbName, DbUserName, DbPassword);
                    }
                    break;
                case ProviderType.Oracle:
                case ProviderType.MsOracle:
                    {
                        if (DbPort <= 0) DbPort = 1521;

                        if (string.IsNullOrEmpty(DbName))
                            DbName = "ORCL";
                        ConnectionString = string.Format("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}))(CONNECT_DATA=(SERVICE_NAME={2})));User Id={3};Password={4};",
                             DbIp, DbPort, DbName, DbUserName, DbPassword);
                    }
                    break;
                case ProviderType.DB2:
                    {
                        if (DbPort <= 0) DbPort = 50000;

                        ConnectionString = string.Format("server={0},{1};database={2};uid={3};pwd={4};",
                            DbIp, DbPort, DbName, DbUserName, DbPassword);
                    }
                    break;
                case ProviderType.MySql:
                    {
                        if (DbPort <= 0) DbPort = 3306;
                        ConnectionString = string.Format("Data Source={0};port={1};Database={2};User Id={3};Password={4};pooling=false;CharSet=utf8;",
                          DbIp, DbPort, DbName, DbUserName, DbPassword);
                    }
                    break;
                case ProviderType.SQLite:
                    ConnectionString = string.Format("Data Source={0};Version=3;Password={1};", DbIp, DbPassword);
                    break;
                case ProviderType.Odbc:
                case ProviderType.OleDb:
                    ConnectionString = "未定义该连接类型字符串";
                    break;
                default:
                    ConnectionString = "未知连接类型";
                    break;
            }
            return ConnectionString;
        }
    }

    public class BaseParameter
    {
        /// <summary>
        /// 超时默认值,1800s
        /// </summary>
        public int ExecuteTimeout { get; set; }

        public BaseParameter(int ExecuteTimeout=60)
        {
            this.ExecuteTimeout = ExecuteTimeout;
        }
    }
}
