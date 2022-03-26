/*-------------------------------------------------------------
 *   auth: bouyei
 *   date: 2017/2/10 9:34:04
 *contact: 453840293@qq.com
 *profile: www.openthinking.cn
 *    Ltd: 
 *   guid: 44ea6ba7-acc1-452e-9c70-58b7a7083a3f
---------------------------------------------------------------*/
using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace Bouyei.DbFactoryCore.DbAdoProvider.Oracle
{

    internal class OracleCopy : BaseFactory
    {
        OracleBulkCopy bulkCopy = null;
        bool disposed = false;

        public BulkCopiedArgs BulkCopiedHandler { get; set; }

        public BulkCopyOptions Option { get; private set; }

        public OracleCopy(string ConnectionString, int timeout = 1800, 
            BulkCopyOptions option = BulkCopyOptions.KeepIdentity)
            :base(FactoryType.Oracle,timeout)
        {
            this.Option = option;
            this.ConnectionString = ConnectionString;
            bulkCopy = CreatedBulkCopy(option);
            bulkCopy.BulkCopyTimeout = timeout;
        }

        public OracleCopy(IDbConnection dbConnection, int timeout = 1800,
            BulkCopyOptions option = BulkCopyOptions.KeepIdentity)
            :base(FactoryType.Oracle,timeout)
        {
            this.Option = option;
            this.ConnectionString = ConnectionString;
            bulkCopy = new OracleBulkCopy((OracleConnection)dbConnection, (OracleBulkCopyOptions)option)
            {
                BulkCopyTimeout = timeout
            };
        }

        private OracleBulkCopy CreatedBulkCopy(BulkCopyOptions option)
        {
            if (option == BulkCopyOptions.Default)
            {
                return new OracleBulkCopy(ConnectionString, OracleBulkCopyOptions.Default);
            }
            else if (option == BulkCopyOptions.UseInternalTransaction)
            {
                return new OracleBulkCopy(ConnectionString, OracleBulkCopyOptions.UseInternalTransaction);
            }
            else return new OracleBulkCopy(ConnectionString);
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
                bulkCopy.OracleRowsCopied += BulkCopy_OracleRowsCopied;
            }
        }

        private void InitBulkCopy(string tableName, string[] columnNames, int batchSize)
        {
            if (bulkCopy.ColumnMappings.Count > 0) bulkCopy.ColumnMappings.Clear();
            bulkCopy.DestinationTableName = tableName;
            bulkCopy.BatchSize = batchSize;

            for (int i = 0; i < columnNames.Length; ++i)
            {
                bulkCopy.ColumnMappings.Add(columnNames[i], columnNames[i]);
            }

            if (BulkCopiedHandler != null)
            {
                bulkCopy.NotifyAfter = batchSize;
                bulkCopy.OracleRowsCopied += BulkCopy_OracleRowsCopied;
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
                bulkCopy.OracleRowsCopied += BulkCopy_OracleRowsCopied;
            }
        }

        public int WriteToServer(DataTable dataSource, int batchSize = 102400)
        {
            InitBulkCopy(dataSource, batchSize);
            bulkCopy.WriteToServer(dataSource);

            return dataSource.Rows.Count;
        }

        public void WriteToServer(IDataReader dataSource, string tableName, int batchSize = 102400)
        {
            string[] columnNames = new string[dataSource.FieldCount];
            for (int i = 0; i < columnNames.Length; ++i)
            {
                columnNames[i] = dataSource.GetName(i);
            }
            InitBulkCopy(tableName, columnNames, batchSize);
            bulkCopy.WriteToServer(dataSource);
        }

        public void WriteToServer(string tableName, DataRow[] rows, int batchSize = 102400)
        {
            InitBulkCopy(tableName, batchSize);
            bulkCopy.WriteToServer(rows);
        }

        void BulkCopy_OracleRowsCopied(object sender, OracleRowsCopiedEventArgs eventArgs)
        {
            if (BulkCopiedHandler != null)
            {
                BulkCopiedHandler(eventArgs.RowsCopied);
            }
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

       public void ReadFromServer<T>(string tableName, Func<T, bool> action)
        {
            throw new Exception("not support");
        }
    }
}
