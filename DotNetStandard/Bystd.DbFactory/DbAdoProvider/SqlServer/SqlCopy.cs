/*-------------------------------------------------------------
 *   auth: bouyei
 *   date: 2017/2/10 9:32:41
 *contact: 453840293@qq.com
 *profile: www.openthinking.cn
 *    Ltd: 
 *   guid: 9d334263-66e6-4107-9e09-361f1b5df138
---------------------------------------------------------------*/
using System;
using System.Data;
using System.Data.SqlClient;

namespace Bystd.DbFactory.DbAdoProvider.SqlServer
{
    internal class SqlCopy:BaseFactory
    {
        SqlBulkCopy bulkCopy = null;
        bool disposed = false;
        
        public BulkCopiedArgs BulkCopiedHandler { get; set; }

        public BulkCopyOptions Option { get;  set; }

        public IDbTransaction dbTrans { get;  set; }

        public IDbConnection dbConnection { get;  set; }

        public SqlCopy(string ConnectionString, int timeout = 1800, 
            BulkCopyOptions option = BulkCopyOptions.KeepIdentity)
            :base(FactoryType.SqlServer,timeout)
        {
            this.Option = option;
            this.ConnectionString = ConnectionString;

            bulkCopy = CreatedBulkCopy(option);
            bulkCopy.BulkCopyTimeout = timeout;
            bulkCopy.EnableStreaming = true;
        }

        public SqlCopy(IDbConnection dbConnection, IDbTransaction dbTrans = null,
            int timeout = 1800, BulkCopyOptions option = BulkCopyOptions.KeepIdentity)
            :base(FactoryType.SqlServer,timeout)
        {
            this.Option = option;
            this.ConnectionString = ConnectionString;
            this.dbTrans = dbTrans;

            SqlConnection connection = (SqlConnection)dbConnection;
            if (dbTrans == null) bulkCopy = new SqlBulkCopy(connection);
            else bulkCopy = new SqlBulkCopy(connection,
                (SqlBulkCopyOptions)option, (SqlTransaction)dbTrans);

            bulkCopy.BulkCopyTimeout = timeout;
            bulkCopy.EnableStreaming = true;
        }

        private SqlBulkCopy CreatedBulkCopy(BulkCopyOptions option)
        {
            if (option == BulkCopyOptions.None)
            {
                return new SqlBulkCopy(ConnectionString);
            }
            else
            {
                return new SqlBulkCopy(ConnectionString, (SqlBulkCopyOptions)option);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                if (bulkCopy != null)
                {
                    bulkCopy.Close();
                    bulkCopy = null;
                }
            }
            disposed = true;
        }

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Close()
        {
            if (bulkCopy != null)
                bulkCopy.Close();
        }

        private void InitBulkCopy(DataTable dataSource, int batchSize)
        {
            if (bulkCopy.ColumnMappings.Count > 0) bulkCopy.ColumnMappings.Clear();

            bulkCopy.ColumnMappings.Capacity = dataSource.Columns.Count;
            bulkCopy.DestinationTableName = dataSource.TableName;
            bulkCopy.BatchSize = batchSize;

            for (int i = 0; i < dataSource.Columns.Count; ++i)
            {
                bulkCopy.ColumnMappings.Add(dataSource.Columns[i].ColumnName,
                    dataSource.Columns[i].ColumnName);
            }
            if (BulkCopiedHandler != null)
            {
                bulkCopy.NotifyAfter = batchSize;
                bulkCopy.SqlRowsCopied += BulkCopy_SqlRowsCopied;
            }
        }

        private void InitBulkCopy(string tableName, string[] columnNames, int batchSize)
        {
            if (bulkCopy.ColumnMappings.Count > 0) bulkCopy.ColumnMappings.Clear();

            bulkCopy.DestinationTableName = tableName;
            bulkCopy.BatchSize = batchSize;

            for (int i = 0; i < columnNames.Length; ++i)
            {
                bulkCopy.ColumnMappings.Add(columnNames[i],
                    columnNames[i]);
            }
            if (BulkCopiedHandler != null)
            {
                bulkCopy.NotifyAfter = batchSize;
                bulkCopy.SqlRowsCopied += BulkCopy_SqlRowsCopied;
            }
        }

        private void InitBulkCopy(string tableName, int batchSize)
        {
            if (bulkCopy.ColumnMappings.Count > 0) bulkCopy.ColumnMappings.Clear();

            bulkCopy.DestinationTableName = tableName;
            bulkCopy.BatchSize = batchSize;

            if (BulkCopiedHandler != null)
            {
                bulkCopy.NotifyAfter = batchSize;
                bulkCopy.SqlRowsCopied += BulkCopy_SqlRowsCopied;
            }
        }

        void BulkCopy_SqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
            if (BulkCopiedHandler != null)
            {
                BulkCopiedHandler(e.RowsCopied);
            }
        }

        public int WriteToServer(DataTable dataSource, int batchSize = 102400)
        {
            InitBulkCopy(dataSource, batchSize);
            bulkCopy.WriteToServer(dataSource);

            return dataSource.Rows.Count;
        }

        public void ReadFromServer<T>(string tableName, Func<T, bool> action)
        {
            throw new Exception("not support");
        }

        public void WriteToServer(IDataReader dataSource, string tableName, int batchSize = 10240)
        {
            string[] columnNames = new string[dataSource.FieldCount];
            for (int i = 0; i < columnNames.Length; ++i)
            {
                columnNames[i] = dataSource.GetName(i);
            }
            InitBulkCopy(tableName, columnNames, batchSize);
            bulkCopy.WriteToServer(dataSource);
        }

        public void WriteToServer(string tableName, DataRow[] rows)
        {
            InitBulkCopy(tableName, rows.Length);
            bulkCopy.WriteToServer(rows);
        }

        public void WriteToServer(DataTable dataSource, DataRowState rowState, int batchSize = 102400)
        {
            InitBulkCopy(dataSource, batchSize);
            bulkCopy.WriteToServer(dataSource, rowState);
        }

        public void WriteToServer(DataRow[] rows, int batchSize = 102400)
        {
            InitBulkCopy(rows[0].Table, batchSize);
            bulkCopy.WriteToServer(rows);
        }
    }
}
