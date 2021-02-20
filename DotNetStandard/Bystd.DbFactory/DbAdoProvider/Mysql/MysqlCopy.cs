/*-------------------------------------------------------------
 *   auth: bouyei
 *   date: 2017/5/20 0:10:20
 *contact: 453840293@qq.com
 *profile: www.openthinking.cn
 *   guid: ba2dd4d8-ebf4-43f9-a2c2-9ede532124ce
---------------------------------------------------------------*/
using System;
using System.Data;
using System.Linq;

namespace Bystd.DbFactory.DbAdoProvider.Mysql
{
    using MySql.Data.MySqlClient;
    using System.IO;
    using System.Reflection;

    internal class MysqlCopy:BaseFactory
    {
        MySqlBulkLoader mysqlBulkCopy = null;

        public MysqlCopy(string ConnectionString, int timeout = 1800)
            :base(FactoryType.MySql,timeout)
        {
            this.ConnectionString = ConnectionString;
            this.ExecuteTimeout = timeout;
        }

        public override void Dispose()
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

        public int WriteToServer(DataTable dataSource, int batchSize = 10240)
        {
            DbUtils.DbCsvAdapter dbCsvHelper = new DbUtils.DbCsvAdapter();
            string path = AppDomain.CurrentDomain.BaseDirectory + dataSource.TableName + DateTime.Now.ToString("yyyyMMddHHmmssfff")+".csv";

            bool isExport = dbCsvHelper.ExportCsvToFile(dataSource, path);
            if (isExport == false) return -1;
             
            int rows = 0;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();

                    mysqlBulkCopy = new MySqlBulkLoader(conn)
                    {
                        Timeout = ExecuteTimeout,
                        TableName = dataSource.TableName,
                        FieldTerminator = ",",
                        FieldQuotationCharacter = '"',
                        LineTerminator = "\r\n",
                        FileName = path,
                        EscapeCharacter = '"',
                        CharacterSet =dbCsvHelper.encoding.BodyName.Replace("-",""),
                        NumberOfLinesToSkip = 1
                    };
                    if (dataSource.Columns != null)
                    {
                        foreach (DataColumn col in dataSource.Columns)
                            mysqlBulkCopy.Columns.Add(col.ColumnName);
                    }
                    rows = mysqlBulkCopy.Load();
                }
            }
            catch(Exception e)
            {
                throw e;
            }
            finally
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            return rows;
        }

        public void ReadFromServer<T>(string tableName, Func<T, bool> action)
        {
            throw new Exception("not support");
        }

        public int WriteToServer(Array dataSource, string tableName, int batchSize = 10240)
        {
            DbUtils.DbCsvAdapter dbCsvHelper = new DbUtils.DbCsvAdapter();
            string path = AppDomain.CurrentDomain.BaseDirectory + DateTime.Now.Ticks+ ".csv";

            bool isExport = dbCsvHelper.ExportCsvToFile(dataSource, path);
            if (isExport == false) return -1;

            int rows = 0;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();

                    mysqlBulkCopy = new MySqlBulkLoader(conn)
                    {
                        Timeout = ExecuteTimeout,
                        TableName = tableName,
                        FieldTerminator = ",",
                        FieldQuotationCharacter = '"',
                        LineTerminator = "\r\n",
                        FileName = path,
                        EscapeCharacter = '"',
                        CharacterSet = dbCsvHelper.encoding.BodyName.Replace("-",""),
                        NumberOfLinesToSkip = 1
                    };

                    var pros = dataSource.GetValue(0).GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

                    mysqlBulkCopy.Columns.AddRange(pros.Select(x => x.Name));

                    rows = mysqlBulkCopy.Load();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            return rows;
        }

        public void WriteToServer(IDataReader dataSource, string tableName, int batchSize = 10240)
        {
            throw new Exception("not support");
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
                    Local=true
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
        public string CharacterSet { get; set; } = "utf8";
        public char EscapeCharacter { get; set; }

        public char FieldQuotationCharacter { get; set; }

        public int NumberOfLinesToSkip { get; set; } = 1;

        public string[] Columns { get; set; }
    }
}
