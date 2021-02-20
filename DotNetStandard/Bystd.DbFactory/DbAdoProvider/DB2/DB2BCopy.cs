/*-------------------------------------------------------------
 *   auth: bouyei
 *   date: 2017/2/10 9:33:53
 *contact: 453840293@qq.com
 *profile: www.openthinking.cn
 *    Ltd: 
 *   guid: fd219fea-b1b9-48b2-b864-2f26d24f678e
---------------------------------------------------------------*/
using System;
using System.Data;
using System.Linq;
using IBM.Data.DB2.Core;

namespace Bystd.DbFactory.DbAdoProvider.DB2
{
    internal class DB2Copy :BaseFactory,IBaseFactory
    {
        DB2BulkCopy bulkCopy = null;
        bool disposed = false;

        public BulkCopyOptions Option { get; private set; }

        public BulkCopiedArgs BulkCopiedHandler { get; set; }

        public DB2Copy(string ConnectionString, int timeout = 1800,
            BulkCopyOptions option = BulkCopyOptions.KeepIdentity)
            :base(FactoryType.DB2,timeout)
        {
            this.Option = option;
            this.ConnectionString = ConnectionString;
            bulkCopy = CreatedBulkCopy(option);
            bulkCopy.BulkCopyTimeout = timeout;
        }

        public DB2Copy(IDbConnection dbConnection, int timeout = 1800,
            BulkCopyOptions option = BulkCopyOptions.KeepIdentity)
            :base(FactoryType.DB2,timeout)
        {
            this.Option = option;
            this.ConnectionString = ConnectionString;
            bulkCopy = new DB2BulkCopy((DB2Connection)dbConnection, (DB2BulkCopyOptions)option)
            {
                BulkCopyTimeout = timeout
            };
        }
        private DB2BulkCopy CreatedBulkCopy(BulkCopyOptions option)
        {
            if (option == BulkCopyOptions.Default ||
                option == BulkCopyOptions.KeepIdentity)
            {
                return new DB2BulkCopy(ConnectionString, (DB2BulkCopyOptions)option);
            }
            else if (option == BulkCopyOptions.TableLock)
            {
                return new DB2BulkCopy(ConnectionString, DB2BulkCopyOptions.TableLock);
            }
            else return new DB2BulkCopy(ConnectionString);
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

            for (int i = 0; i < dataSource.Columns.Count; ++i)
            {
                bulkCopy.ColumnMappings.Add(dataSource.Columns[i].ColumnName,
                    dataSource.Columns[i].ColumnName);
            }
            if (BulkCopiedHandler != null)
            {
                bulkCopy.NotifyAfter = batchSize;
                bulkCopy.DB2RowsCopied += BulkCopy_DB2RowsCopied;
            }
        }

        private void InitBulkCopy(string tableName, string[] columnNames, int batchSize)
        {
            if (bulkCopy.ColumnMappings.Count > 0) bulkCopy.ColumnMappings.Clear();

            bulkCopy.DestinationTableName = tableName;
            for (int i = 0; i < columnNames.Length; ++i)
            {
                bulkCopy.ColumnMappings.Add(columnNames[i],
                    columnNames[i]);
            }

            if (BulkCopiedHandler != null)
            {
                bulkCopy.NotifyAfter = batchSize;
                bulkCopy.DB2RowsCopied += BulkCopy_DB2RowsCopied;
            }
        }

        private void InitBulkCopy(string tableName, int batchSize)
        {
            if (bulkCopy.ColumnMappings.Count > 0) bulkCopy.ColumnMappings.Clear();

            bulkCopy.DestinationTableName = tableName;

            if (BulkCopiedHandler != null)
            {
                bulkCopy.NotifyAfter = batchSize;
                bulkCopy.DB2RowsCopied += BulkCopy_DB2RowsCopied;
            }
        }

        void BulkCopy_DB2RowsCopied(object sender, DB2RowsCopiedEventArgs e)
        {
            if (BulkCopiedHandler != null)
            {
                BulkCopiedHandler(e.RowsCopied);
            }
        }

        public int WriteToServer(DataTable dataSource, int batchSize = 10240)
        {
            InitBulkCopy(dataSource, batchSize);
            bulkCopy.WriteToServer(dataSource);

            if (bulkCopy.Errors.Count > 0)
            {
                throw new Exception(string.Format("入库失败条数:{0}信息;{1}", bulkCopy.Errors.Count, bulkCopy.Errors[0].Message));
            }

            return dataSource.Rows.Count;
        }

        public void WriteToServer(IDataReader dataSource,string tableName, int batchSize = 10240)
        {
            string[] columnNames = new string[dataSource.FieldCount];
            for (int i = 0; i < columnNames.Length; ++i)
            {
                columnNames[i] = dataSource.GetName(i);
            }
            InitBulkCopy(tableName, columnNames, batchSize);
            bulkCopy.WriteToServer(dataSource);
            if (bulkCopy.Errors.Count > 0)
            {
                throw new Exception(string.Format("入库失败条数:{0}信息;{1}", bulkCopy.Errors.Count, bulkCopy.Errors[0].Message));
            }
        }

        public void WriteToServer(string tableName, DataRow[] rows, int batchSize = 10240)
        {
            InitBulkCopy(tableName, batchSize);

            bulkCopy.WriteToServer(rows);
            if (bulkCopy.Errors.Count > 0)
            {
                throw new Exception(string.Format("入库失败条数:{0}信息;{1}", bulkCopy.Errors.Count, bulkCopy.Errors[0].Message));
            }
        }

        public void WriteToServer(DataTable dataSource, DataRowState rowState, int batchSize = 10240)
        {
            InitBulkCopy(dataSource, batchSize);
            bulkCopy.WriteToServer(dataSource, rowState);

            if (bulkCopy.Errors.Count > 0)
            {
                throw new Exception(string.Format("入库失败条数:{0}信息;{1}", bulkCopy.Errors.Count, bulkCopy.Errors[0].Message));
            }
        }

        public void WriteToServer(DataRow[] rows, int batchSize = 10240)
        {
            InitBulkCopy(rows[0].Table, batchSize);
            bulkCopy.WriteToServer(rows);

            if (bulkCopy.Errors.Count > 0)
            {
                throw new Exception(string.Format("入库失败条数:{0}信息;{1}", bulkCopy.Errors.Count, bulkCopy.Errors[0].Message));
            }
        }

        public void ReadFromServer<T>(string tableName, Func<T, bool> action)
        {
            throw new Exception("not support");
        }
    }
}
