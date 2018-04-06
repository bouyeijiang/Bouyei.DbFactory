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

namespace Bouyei.DbFactoryCore.DbAdoProvider
{
    using Plugins;

    public class DbCommonBulkCopy : IDbBulkCopy
    {
        #region public field
                public BulkCopiedArgs BulkCopiedHandler { get; set; }

                public string DestinationTableName { get;private set; }

                public int BulkCopyTimeout { get; set; }

                public int BatchSize { get; set; }

                public string ConnectionString { get; private set; }

                public ProviderType ProviderName { get; private set; }

                public BulkCopyOptions DbBulkCopyOption { get; set; }

                public bool IsTransaction { get; private set; }

                public IDbTransaction dbTrans { get; private set; }

                public IDbConnection dbConn { get; private set; }
        #endregion

        SqlBulk sqlBulkCopy = null;
        NpgBulk npgBulkCopy = null;
        //Db2Bulk db2BulkCopy = null;
        //OracleBulk oracleBulkCopy = null;
        MysqlBulk mySqlBulkCopy = null;

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
                if (ProviderName == ProviderType.SqlServer) sqlBulkCopy.Dispose();
                if (ProviderName == ProviderType.PostgreSQL) npgBulkCopy.Dispose();
                //else if (ProviderName == ProviderType.DB2) db2BulkCopy.Dispose();
                //else if (ProviderName == ProviderType.Oracle) oracleBulkCopy.Dispose();
                else if (ProviderName == ProviderType.MySql) mySqlBulkCopy.Dispose();
            }
        }

        protected DbCommonBulkCopy(ProviderType providerType,
           string connectionString)
        {
            this.ConnectionString = connectionString;
            this.ProviderName = providerType;
        }

        public DbCommonBulkCopy(ProviderType providerType, 
            string connectionString,
            int bulkcopyTimeout = 1800,
            int batchSize = 102400,
            BulkCopyOptions dbBulkCopyOption = BulkCopyOptions.KeepIdentity)
            : this(providerType, connectionString)
        {
            this.BatchSize = batchSize;
            this.BulkCopyTimeout = bulkcopyTimeout;
            this.DbBulkCopyOption = dbBulkCopyOption;

            if (ProviderName == ProviderType.SqlServer)
            {
                if (sqlBulkCopy == null || this.ConnectionString != ConnectionString)
                {
                    sqlBulkCopy = new SqlBulk(ConnectionString, BulkCopyTimeout, DbBulkCopyOption);
                }
            }
            //else if (ProviderName == ProviderType.DB2)
            //{
            //    if (db2BulkCopy == null || this.ConnectionString != ConnectionString)
            //    {
            //        db2BulkCopy = new Db2Bulk(ConnectionString, BulkCopyTimeout, DbBulkCopyOption);
            //    }
            //}
            //else if (ProviderName == ProviderType.Oracle)
            //{
            //    if (oracleBulkCopy == null || this.ConnectionString != ConnectionString)
            //    {
            //        oracleBulkCopy = new OracleBulk(ConnectionString, BulkCopyTimeout, DbBulkCopyOption);
            //    }
            //}
            else if (ProviderName == ProviderType.MySql)
            {
                mySqlBulkCopy = new MysqlBulk(ConnectionString, BulkCopyTimeout);
            }
            else if (providerType == ProviderType.PostgreSQL)
            {
                npgBulkCopy = new NpgBulk(ConnectionString, bulkcopyTimeout);
            }
        }

        public DbCommonBulkCopy(ProviderType providerType,
            string connectionString, 
            IDbConnection dbConnection,
            int bulkcopyTimeout = 1800,
            int batchSize = 102400,
            BulkCopyOptions dbBulkCopyOption = BulkCopyOptions.KeepIdentity,
            bool isTransaction = true)
            : this(providerType, connectionString)
        {
            this.BatchSize = batchSize;
            this.BulkCopyTimeout = bulkcopyTimeout;
            this.DbBulkCopyOption = dbBulkCopyOption;
            this.IsTransaction = isTransaction;
            this.dbConn = dbConnection;

            if (ProviderName == ProviderType.SqlServer)
            {
                if (sqlBulkCopy != null || this.ConnectionString != connectionString)
                {
                    if (sqlBulkCopy != null)
                        sqlBulkCopy.Dispose();
                }
                if (dbConn.State != ConnectionState.Open) dbConn.Open();

                if (IsTransaction)
                {
                    dbTrans = dbConn.BeginTransaction();
                    DbBulkCopyOption = BulkCopyOptions.UseInternalTransaction; 
                }
                sqlBulkCopy = new SqlBulk(dbConn, dbTrans, BulkCopyTimeout, DbBulkCopyOption);
            }
            //else if (ProviderName == ProviderType.DB2)
            //{
            //    if (db2BulkCopy != null || this.ConnectionString != connectionString)
            //    {
            //        if (db2BulkCopy != null)
            //            db2BulkCopy.Dispose();
            //    }

            //    if (dbConn.State != ConnectionState.Open) dbConn.Open();

            //    if (isTransaction)
            //    {
            //        DbBulkCopyOption = BulkCopyOptions.UseInternalTransaction;
            //    }
            //    db2BulkCopy = new Db2Bulk(dbConn, BulkCopyTimeout, DbBulkCopyOption);
            //}
            //else if (ProviderName == ProviderType.Oracle)
            //{
            //    if (oracleBulkCopy != null || this.ConnectionString != connectionString)
            //    {
            //        if (oracleBulkCopy != null)
            //            oracleBulkCopy.Dispose();
            //    }

            //    if (dbConn.State != ConnectionState.Open) dbConn.Open();

            //    if (isTransaction)
            //    {
            //        DbBulkCopyOption = BulkCopyOptions.UseInternalTransaction;
            //    }

            //    oracleBulkCopy = new OracleBulk(dbConn, BulkCopyTimeout, DbBulkCopyOption);
            //}
            else if (ProviderName == ProviderType.MySql)
            {
                if(mySqlBulkCopy!=null || this.ConnectionString != connectionString)
                {
                    if (mySqlBulkCopy != null)
                        mySqlBulkCopy.Dispose();
                }
                mySqlBulkCopy = new MysqlBulk(ConnectionString, BulkCopyTimeout);
            }
            else if (providerType == ProviderType.PostgreSQL)
            {
                npgBulkCopy = new NpgBulk(ConnectionString, bulkcopyTimeout);
            }
        }

        public void Close()
        {
            if (ProviderName == ProviderType.SqlServer) sqlBulkCopy.Close();
            else if (ProviderName == ProviderType.MySql) mySqlBulkCopy.Close();
            //else if (ProviderName == ProviderType.DB2) db2BulkCopy.Close();
            //else if (ProviderName == ProviderType.Oracle) oracleBulkCopy.Close();
        }

        public void Open()
        {
            if (!IsTransaction) return;

            if (dbConn.State != ConnectionState.Open) dbConn.Open();
        }

        public void WriteToServer(DataTable sourceTable)
        {
            DestinationTableName = sourceTable.TableName;
            if (ProviderName == ProviderType.SqlServer)
            {
                sqlBulkCopy.BulkCopiedHandler = BulkCopiedHandler;
                sqlBulkCopy.WriteToServer(sourceTable, BatchSize);
            }
            //else if (ProviderName == ProviderType.DB2)
            //{
            //    db2BulkCopy.BulkCopiedHandler = BulkCopiedHandler;
            //    db2BulkCopy.WriteToServer(sourceTable,BatchSize);
            //}
            //else if (ProviderName == ProviderType.Oracle)
            //{
            //    oracleBulkCopy.BulkCopiedHandler = BulkCopiedHandler;
            //    oracleBulkCopy.WriteToServer(sourceTable, BatchSize);
            //}
            else if (ProviderName == ProviderType.MySql)
            {
                DbUtils.DbCsvHelper csv = new DbUtils.DbCsvHelper();
                string fname = sourceTable.TableName + DateTime.Now.Ticks;
                bool rt = csv.ExportSvcToFile(sourceTable, fname);
                if (rt == false) return;

               int rows= mySqlBulkCopy.WriteToServer(new MysqlBulkLoaderInfo()
                {
                    FileName = fname,
                    FieldTerminator = ",",
                    LineTerminator = "\r\n",
                    TableName = sourceTable.TableName,
                    FieldQuotationCharacter = '"',
                    EscapeCharacter = '"',
                });

                if (rows == 0) throw new Exception("导入空数据...");
            }
            else if (ProviderName == ProviderType.PostgreSQL)
            {
                npgBulkCopy.WriteToServer(sourceTable);
            }
            else
            {
                throw new Exception("暂时不支持" + ProviderName.ToString() + "的批量方法...");
            }
        }

        public void WriteToServer(DataTable sourceTable, DataRowState rowState)
        {
            DestinationTableName = sourceTable.TableName;
            if (ProviderName == ProviderType.SqlServer)
            {
                sqlBulkCopy.BulkCopiedHandler = BulkCopiedHandler;
                sqlBulkCopy.WriteToServer(sourceTable, rowState, BatchSize);
            }
            //else if (ProviderName == ProviderType.DB2)
            //{
            //    db2BulkCopy.BulkCopiedHandler = BulkCopiedHandler;
            //    db2BulkCopy.WriteToServer(sourceTable, rowState);
            //}
            //else if (ProviderName == ProviderType.Oracle)
            //{
            //    oracleBulkCopy.BulkCopiedHandler = BulkCopiedHandler;
            //    oracleBulkCopy.WriteToServer(sourceTable, rowState, BatchSize);
            //}
            else
            {
                throw new Exception("暂时不支持" + ProviderName.ToString() + "的批量方法...");
            }
        }

        public void WriteToServer(IDataReader iDataReader, string sourceTableName)
        {
            DestinationTableName = sourceTableName;
            if (ProviderName == ProviderType.SqlServer)
            {
                sqlBulkCopy.BulkCopiedHandler = BulkCopiedHandler;
                sqlBulkCopy.WriteToServer(sourceTableName, iDataReader, this.BatchSize);
            }
            //else if (ProviderName == ProviderType.DB2)
            //{
            //    db2BulkCopy.BulkCopiedHandler = BulkCopiedHandler;
            //    db2BulkCopy.WriteToServer(sourceTableName, iDataReader);
            //}
            //else if (ProviderName == ProviderType.Oracle)
            //{
            //    oracleBulkCopy.BulkCopiedHandler = BulkCopiedHandler;
            //    oracleBulkCopy.WriteToServer(sourceTableName, iDataReader, BatchSize);
            //}
            else
            {
                throw new Exception("暂时不支持" + ProviderName.ToString() + "的批量方法...");
            }
        }
    }
}
