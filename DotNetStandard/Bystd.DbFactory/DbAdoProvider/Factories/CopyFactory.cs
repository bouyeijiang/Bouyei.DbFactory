using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bystd.DbFactory.DbAdoProvider
{
    public class CopyFactory : ICopyFactory
    {
        public BulkCopiedArgs BulkCopiedHandler { get; set; }

        public FactoryType DbType { get; set; }

        public int ExecuteTimeout { get; set; }

        public string ConnectionString { get; set; }

        public BulkCopyOptions Options { get; set; }

        public CopyFactory(FactoryType dbType,string ConnectionString,
           int executeTimeout, BulkCopyOptions options = BulkCopyOptions.KeepIdentity)
        {
            this.DbType = dbType;
            this.ConnectionString = ConnectionString;
            this.ExecuteTimeout = executeTimeout;
            this.Options = options;
        }

        public int WriteToServer(DataTable dataSource, int batchSize = 10240)
        {
            if (DbType == FactoryType.PostgreSQL)
            {
                using (var copy = new Postgresql.NpgCopy(ConnectionString, ExecuteTimeout))
                {
                    return copy.WriteToServer(dataSource, batchSize);
                }
            }
            else if (DbType == FactoryType.SqlServer)
            {
                using (var copy = new SqlServer.SqlCopy(ConnectionString, ExecuteTimeout,Options))
                {
                    copy.BulkCopiedHandler = BulkCopiedHandler;
                    return copy.WriteToServer(dataSource, batchSize);
                }
            }
            else if (DbType == FactoryType.Oracle)
            {
                using (var copy = new Oracle.OracleCopy(ConnectionString, ExecuteTimeout,Options))
                {
                    copy.BulkCopiedHandler = BulkCopiedHandler;
                    return copy.WriteToServer(dataSource, batchSize);
                }
            }
            else if (DbType == FactoryType.MySql)
            {
                using (var copy = new Mysql.MysqlCopy(ConnectionString, ExecuteTimeout))
                {
                    return copy.WriteToServer(dataSource, batchSize);
                }
            }
            else if (DbType == FactoryType.DB2)
            {
                using (var copy = new DB2.DB2Copy(ConnectionString, ExecuteTimeout,Options))
                {
                    copy.BulkCopiedHandler = BulkCopiedHandler;
                    return copy.WriteToServer(dataSource, batchSize);
                }
            }
            else throw new Exception("not supported factory type");
        }

        public int WriteToServer(Array dataSource, string tableName, int batchSize = 1024)
        {
            if (DbType == FactoryType.PostgreSQL)
            {
                using (var copy = new Postgresql.NpgCopy(ConnectionString, ExecuteTimeout))
                {
                    return copy.WriteToServer(dataSource,tableName, batchSize);
                }
            }
            else if (DbType == FactoryType.SqlServer)
            {
                var data = ArrayToDataTable(dataSource, tableName);

                using (var copy = new SqlServer.SqlCopy(ConnectionString, ExecuteTimeout, Options))
                {
                    copy.BulkCopiedHandler = BulkCopiedHandler;
                    return copy.WriteToServer(data, batchSize);
                }
            }
            else if (DbType == FactoryType.Oracle)
            {
                var data = ArrayToDataTable(dataSource, tableName);

                using (var copy = new Oracle.OracleCopy(ConnectionString, ExecuteTimeout, Options))
                {
                    copy.BulkCopiedHandler = BulkCopiedHandler;
                    return copy.WriteToServer(data, batchSize);
                }
            }
            else if (DbType == FactoryType.MySql)
            {
                using (var copy = new Mysql.MysqlCopy(ConnectionString, ExecuteTimeout))
                {
                    return copy.WriteToServer(dataSource,tableName, batchSize);
                }
            }
            else if (DbType == FactoryType.DB2)
            {
                var data = ArrayToDataTable(dataSource, tableName);
                using (var copy = new DB2.DB2Copy(ConnectionString, ExecuteTimeout, Options))
                {
                    copy.BulkCopiedHandler = BulkCopiedHandler;
                    return copy.WriteToServer(data, batchSize);
                }
            }
            else throw new Exception("not supported factory type");
        }

        public void WriteToServer(IDataReader dataSource, string tableName, int batchSize = 10240)
        {
            if (DbType == FactoryType.PostgreSQL)
            {
                using (var copy = new Postgresql.NpgCopy(ConnectionString, ExecuteTimeout))
                {
                    copy.WriteToServer(dataSource,tableName, batchSize);
                }
            }
            else if (DbType == FactoryType.SqlServer)
            {
                using (var copy = new SqlServer.SqlCopy(ConnectionString, ExecuteTimeout, Options))
                {
                    copy.BulkCopiedHandler = BulkCopiedHandler;
                    copy.WriteToServer(dataSource,tableName, batchSize);
                }
            }
            else if (DbType == FactoryType.Oracle)
            {
                using (var copy = new Oracle.OracleCopy(ConnectionString, ExecuteTimeout, Options))
                {
                    copy.BulkCopiedHandler = BulkCopiedHandler;
                    copy.WriteToServer(dataSource,tableName, batchSize);
                }
            }
            else if (DbType == FactoryType.MySql)
            {
                using (var copy = new Mysql.MysqlCopy(ConnectionString, ExecuteTimeout))
                {
                      copy.WriteToServer(dataSource,tableName, batchSize);
                }
            }
            else if (DbType == FactoryType.DB2)
            {
                using (var copy = new DB2.DB2Copy(ConnectionString, ExecuteTimeout, Options))
                {
                    copy.BulkCopiedHandler = BulkCopiedHandler;
                    copy.WriteToServer(dataSource,tableName, batchSize);
                }
            }
            else throw new Exception("not supported factory type");
        }

        public void ReadFromServer<T>(string tableName, Func<T, bool> action)
        {
            throw new NotImplementedException();
        }

        private DataTable ArrayToDataTable(Array dataSource, string tableName)
        {
            var firstRow = dataSource.GetValue(0).GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            DataTable dt = new DataTable();
            dt.TableName = tableName;

            foreach (var p in firstRow)
            {
                dt.Columns.Add(p.Name, p.PropertyType);
            }

            foreach (var row in dataSource)
            {
                // var types = row.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

                var vals = firstRow.Select(x => x.GetValue(row, null)).Where(x => x != null).ToArray();
                dt.Rows.Add(vals);
            }

            return dt;
        }

    }
}
