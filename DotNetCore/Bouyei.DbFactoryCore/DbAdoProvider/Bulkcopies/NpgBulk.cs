using System;
using System.Linq;
using System.Data;

using Npgsql;
namespace Bouyei.DbFactoryCore.DbAdoProvider.Bulkcopies
{
    using Factories;
    internal class NpgBulk : BaseFactory, IFactory
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public NpgBulk() { }

        public NpgBulk(string ConnectionString, int timeout = 1800)
        {
            this.ConnectionString = ConnectionString;
            this.ExecuteTimeout = timeout;
        }

        public int WriteToServer(DataTable dataSource,int batchSize=10240)
        {
            int rows = 0;
            using (NpgsqlConnection conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                using (var import = conn.BeginBinaryImport(ToImportFormat(dataSource)))
                {
                    foreach (DataRow dr in dataSource.Rows)
                    {
                        //import.StartRow();
                        import.WriteRow(dr.ItemArray);
                        ++rows;
                    }
                }
            }
            return rows;
        }

        public void WriteToServer(IDataReader reader,string tableName,int batchSize = 10240)
        {
            throw new Exception("no support");
        }

        public void ReadFromServer<T>(string tableName, Func<T, bool> action)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                using (var export = conn.BeginBinaryExport(string.Format("COPY {0}({1}) TO STDOUT (FORMAT BINARY)", tableName, "")))
                {
                    BinaryToList<T>(export, action);
                }
            }
        }

        private string ToImportFormat(DataTable dataSource)
        {
            return string.Format("COPY {0}({1})  FROM STDIN(FORMAT BINARY)", dataSource.TableName,
                string.Join(",", dataSource.Columns.Cast<DataColumn>().Select(x => x.ColumnName)));
        }

        private void BinaryToList<T>(NpgsqlBinaryExporter exporter, Func<T, bool> rowAction)
        {
            var type = typeof(T);
            var items = type.GetProperties(System.Reflection.BindingFlags.Public |
                 System.Reflection.BindingFlags.Instance);
            while (exporter.StartRow() > 0)
            {
                T value = Activator.CreateInstance<T>();
                foreach (var item in items)
                {
                    if (exporter.IsNull) continue;

                    var pro = type.GetProperty(item.Name);
                    var proType = pro.PropertyType;

                    var rValue = ReadValue(proType.Name, exporter);
                    if (rValue == null) continue;

                    pro.SetValue(value, rValue);
                }
                bool isContinue = rowAction(value);

                if (isContinue == false)
                {
                    exporter.Cancel();
                    break;
                }
            }
        }

        private object ReadValue(string typeName, NpgsqlBinaryExporter exporter)
        {
            switch (typeName)
            {
                case "String": return exporter.Read<string>();
                case "Int":
                case "Int32": return exporter.Read<Int32>();
                case "Long":
                case "Int64": return exporter.Read<Int64>();
                case "Double": return exporter.Read<double>();
                case "Float": return exporter.Read<float>();
                case "Byte": return exporter.Read<byte>();
                case "Boolean": return exporter.Read<bool>();
            }
            return null;
        }
    }
}
