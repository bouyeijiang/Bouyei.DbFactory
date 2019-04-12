/*-------------------------------------------------------------
 *   auth: bouyei
 *   date: 2017/5/20 0:10:20
 *contact: 453840293@qq.com
 *profile: www.openthinking.cn
 *   guid: ba2dd4d8-ebf4-43f9-a2c2-9ede532124ce
---------------------------------------------------------------*/
using System;
using System.Data;

namespace Bouyei.DbFactory.DbAdoProvider.Bulkcopies
{
    using Factories;
    using MySql.Data.MySqlClient;

    internal class MysqlBulk:BaseFactory,IFactory
    {
        MySqlBulkLoader mysqlBulkCopy = null;

        public MysqlBulk()
        { }
        public MysqlBulk(string ConnectionString, int timeout = 1800)
        {
            this.ConnectionString = ConnectionString;
            this.ExecuteTimeout = timeout;
        }

        public void Dispose()
        {
            if (mysqlBulkCopy != null)
            {
                if (mysqlBulkCopy.Connection != null)
                {
                    mysqlBulkCopy.Connection.Dispose();
                }
                mysqlBulkCopy = null;
            }
        }

        public void Close()
        {
            if (mysqlBulkCopy != null
                && mysqlBulkCopy.Connection != null)
                mysqlBulkCopy.Connection.Close();
        }

        public int WriteToServer(DataTable dt, int batchSize = 10240)
        {
            DbUtils.DataCsvAdapter dbCsvHelper = new DbUtils.DataCsvAdapter();
            string path = AppDomain.CurrentDomain.BaseDirectory + dt.TableName;

            bool isExport = dbCsvHelper.ExportSvcToFile(dt, path);
            if (isExport == false) return -1;


            int writedCnt = 0;

            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();

                mysqlBulkCopy = new MySqlBulkLoader(conn)
                {
                    Timeout = ExecuteTimeout,
                    TableName = dt.TableName,
                    FieldTerminator = ",",
                    FieldQuotationCharacter = '"',
                    LineTerminator = "\r\n",
                    FileName = path,
                    EscapeCharacter = '"',
                    CharacterSet = "utf-8",
                    NumberOfLinesToSkip = 0,
                };
                if (dt.Columns != null)
                {
                    foreach (DataColumn col in dt.Columns)
                        mysqlBulkCopy.Columns.Add(col.ColumnName);
                }
                writedCnt = mysqlBulkCopy.Load();
            }

            System.IO.File.Delete(path);

            return writedCnt;
        }

        public void ReadFromServer<T>(string tableName, Func<T, bool> action)
        {
            throw new Exception("no support");
        }

        public void WriteToServer(IDataReader reader, string tableName, int batchSize = 10240)
        {
            throw new Exception("no support");
        }

        public int WriteToServer(MysqlBulkLoaderInfo bulkLoaderInfo)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();

                mysqlBulkCopy = new MySqlBulkLoader(conn)
                {
                    Timeout = ExecuteTimeout,
                    TableName = bulkLoaderInfo.TableName,
                    FieldTerminator = bulkLoaderInfo.FieldTerminator,
                    LineTerminator = bulkLoaderInfo.LineTerminator,
                    LinePrefix = bulkLoaderInfo.LinePrefix,
                    FileName = bulkLoaderInfo.FileName,
                    FieldQuotationCharacter = bulkLoaderInfo.FieldQuotationCharacter,
                    EscapeCharacter = bulkLoaderInfo.EscapeCharacter,
                    CharacterSet = bulkLoaderInfo.CharacterSet,
                    NumberOfLinesToSkip = bulkLoaderInfo.NumberOfLinesToSkip,
                };
                if (bulkLoaderInfo.Columns != null)
                {
                    mysqlBulkCopy.Columns.AddRange(bulkLoaderInfo.Columns);
                }
                return mysqlBulkCopy.Load();
            }
        }
    }

    public class MysqlBulkLoaderInfo
    {
        public string TableName { get; set; }
        public string FieldTerminator { get; set; } = "\t";
        public string LineTerminator { get; set; } = "\n";
        public string LinePrefix { get; set; }
        public string FileName { get; set; }
        public string CharacterSet { get; set; } = "utf-8";
        public char EscapeCharacter { get; set; }

        public char FieldQuotationCharacter { get; set; }

        public int NumberOfLinesToSkip { get; set; } = 1;

        public string[] Columns { get; set; }
    }
}
