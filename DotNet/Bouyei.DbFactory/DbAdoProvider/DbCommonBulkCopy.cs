/*-------------------------------------------------------------
 *   auth: bouyei
 *   date: 2016/7/12 11:59:12
 *contact: 453840293@qq.com
 *profile: www.openthinking.cn
 *    Ltd: Microsoft
 *   guid: 83d74724-7c1b-4d29-be1e-b758a8a2f17c
---------------------------------------------------------------*/
using System;
using System.Data;
using System.Linq;

namespace Bouyei.DbFactory.DbAdoProvider
{
    using Factories;
    using System.Reflection;

    public class DbCommonBulkCopy : IDbBulkCopy
    {
        #region public field
        public BulkCopiedArgs BulkCopiedHandler { get; set; }

        public string DestinationTableName { get; private set; }

        public int BulkCopyTimeout { get; set; }

        public int BatchSize { get; set; }

        public string ConnectionString { get; private set; }

        public DbType DbType { get; private set; }

        public BulkCopyOptions DbBulkCopyOption { get; set; }

        public bool IsTransaction { get; private set; }

        public IDbTransaction dbTrans { get; private set; }

        public IDbConnection dbConn { get; private set; }
        #endregion

        // SqlBulk sqlBulkCopy = null;
        //Db2Bulk db2BulkCopy = null;
        //OracleBulk oracleBulkCopy = null;
        //MysqlBulk mySqlBulkCopy = null;
        //NpgBulk npgBulkCopy = null;
        IFactory factory = null;

        ~DbCommonBulkCopy()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                factory.Dispose();
            }
        }

        protected DbCommonBulkCopy(DbType dbType,
           string connectionString)
        {
            this.ConnectionString = connectionString;
            this.DbType = dbType;
        }

        public DbCommonBulkCopy(DbType dbType,
            string connectionString,
            int bulkcopyTimeout = 1800,
            int batchSize = 102400,
            BulkCopyOptions dbBulkCopyOption = BulkCopyOptions.KeepIdentity)
            : this(dbType, connectionString)
        {
            this.BatchSize = batchSize;
            this.BulkCopyTimeout = bulkcopyTimeout;
            this.DbBulkCopyOption = dbBulkCopyOption;

            if (DbType == DbType.SqlServer)
            {
                if (factory == null || this.ConnectionString != ConnectionString)
                {
                    factory = new SqlFactory(ConnectionString, BulkCopyTimeout, DbBulkCopyOption);
                }
            }
            else if (DbType == DbType.DB2)
            {
                if (factory == null || this.ConnectionString != ConnectionString)
                {
                    factory = new Db2Factory(ConnectionString, BulkCopyTimeout, DbBulkCopyOption);
                }
            }
            else if (DbType == DbType.Oracle)
            {
                if (factory == null || this.ConnectionString != ConnectionString)
                {
                    factory = new OracleFactory(ConnectionString, BulkCopyTimeout, DbBulkCopyOption);
                }
            }
            else if (DbType == DbType.MySql)
            {
                factory = new MysqlFactory(ConnectionString, BulkCopyTimeout);
            }
            else if (DbType == DbType.PostgreSQL)
            {
                factory = new NpgFactory(ConnectionString, bulkcopyTimeout);
            }
        }

        public DbCommonBulkCopy(DbType dbType,
            string connectionString,
            IDbConnection dbConnection,
            int bulkcopyTimeout = 1800,
            int batchSize = 102400,
            BulkCopyOptions dbBulkCopyOption = BulkCopyOptions.KeepIdentity,
            bool isTransaction = true)
            : this(dbType, connectionString)
        {
            this.BatchSize = batchSize;
            this.BulkCopyTimeout = bulkcopyTimeout;
            this.DbBulkCopyOption = dbBulkCopyOption;
            this.IsTransaction = isTransaction;
            this.dbConn = dbConnection;

            if (DbType == DbType.SqlServer)
            {
                if (factory != null || this.ConnectionString != connectionString)
                {
                    if (factory != null)
                        factory.Dispose();
                }
                if (dbConn.State != ConnectionState.Open) dbConn.Open();

                if (IsTransaction)
                {
                    dbTrans = dbConn.BeginTransaction();
                    DbBulkCopyOption = BulkCopyOptions.UseInternalTransaction;
                }
                factory = new SqlFactory(dbConn, dbTrans, BulkCopyTimeout, DbBulkCopyOption);
            }
            else if (DbType == DbType.DB2)
            {
                if (factory != null || this.ConnectionString != connectionString)
                {
                    if (factory != null)
                        factory.Dispose();
                }

                if (dbConn.State != ConnectionState.Open) dbConn.Open();

                if (isTransaction)
                {
                    DbBulkCopyOption = BulkCopyOptions.UseInternalTransaction;
                }
                factory = new Db2Factory(dbConn, BulkCopyTimeout, DbBulkCopyOption);
            }
            else if (DbType == DbType.Oracle)
            {
                if (factory != null || this.ConnectionString != connectionString)
                {
                    if (factory != null)
                        factory.Dispose();
                }

                if (dbConn.State != ConnectionState.Open) dbConn.Open();

                if (isTransaction)
                {
                    DbBulkCopyOption = BulkCopyOptions.UseInternalTransaction;
                }

                factory = new OracleFactory(dbConn, BulkCopyTimeout, DbBulkCopyOption);
            }
            else if (DbType == DbType.MySql)
            {
                if (factory != null || this.ConnectionString != connectionString)
                {
                    if (factory != null)
                        factory.Dispose();
                }
                factory = new MysqlFactory(ConnectionString, BulkCopyTimeout);
            }
            else if (DbType == DbType.PostgreSQL)
            {
                factory = new NpgFactory(ConnectionString, bulkcopyTimeout);
            }
        }

        public void Open()
        {
            if (!IsTransaction) return;

            if (dbConn.State != ConnectionState.Open) dbConn.Open();
        }

        public void WriteToServer(DataTable sourceTable)
        {
            DestinationTableName = sourceTable.TableName;
            factory.WriteToServer(sourceTable, BatchSize);
        }
         
        public void WriteToServer(Array sourceTable, string tabName)
        {
            DestinationTableName = tabName;
            factory.WriteToServer(sourceTable, tabName, BatchSize);
        }

        public void WriteToServer(IDataReader iDataReader, string tableName)
        {
            DestinationTableName = tableName;

            factory.WriteToServer(iDataReader, tableName, BatchSize);
        }
    }
}
